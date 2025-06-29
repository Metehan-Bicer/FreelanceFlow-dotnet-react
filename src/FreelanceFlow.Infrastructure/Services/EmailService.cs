using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using FreelanceFlow.Application.DTOs.Invoice;
using FreelanceFlow.Application.Services.Interfaces;
using FreelanceFlow.Core.Common;

namespace FreelanceFlow.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Result<string>> SendEmailAsync(string to, string subject, string body, byte[]? attachment = null, string? attachmentName = null)
    {
        try
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var message = new MimeMessage();
            
            message.From.Add(new MailboxAddress(
                emailSettings["SenderName"], 
                emailSettings["SenderEmail"]));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };

            if (attachment != null && !string.IsNullOrEmpty(attachmentName))
            {
                builder.Attachments.Add(attachmentName, attachment);
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                emailSettings["SmtpServer"], 
                int.Parse(emailSettings["SmtpPort"]), 
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                emailSettings["SenderEmail"], 
                emailSettings["SenderPassword"]);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return Result<string>.Success("Email başarıyla gönderildi.");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Email gönderilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<string>> SendInvoiceEmailAsync(string to, InvoiceDto invoice, byte[] pdfAttachment)
    {
        var subject = $"Fatura #{invoice.InvoiceNumber} - {invoice.ClientName}";
        
        var body = $@"
            <html>
            <body>
                <h2>Sayın {invoice.ClientName},</h2>
                <p>Faturanız hazırlanmıştır. Detaylar aşağıdaki gibidir:</p>
                
                <table style='border-collapse: collapse; width: 100%;'>
                    <tr>
                        <td style='border: 1px solid #ddd; padding: 8px; font-weight: bold;'>Fatura No:</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{invoice.InvoiceNumber}</td>
                    </tr>
                    <tr>
                        <td style='border: 1px solid #ddd; padding: 8px; font-weight: bold;'>Tarih:</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{invoice.IssueDate:dd.MM.yyyy}</td>
                    </tr>
                    <tr>
                        <td style='border: 1px solid #ddd; padding: 8px; font-weight: bold;'>Vade Tarihi:</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{invoice.DueDate:dd.MM.yyyy}</td>
                    </tr>
                    <tr>
                        <td style='border: 1px solid #ddd; padding: 8px; font-weight: bold;'>Toplam Tutar:</td>
                        <td style='border: 1px solid #ddd; padding: 8px; font-weight: bold; color: #2E8B57;'>{invoice.TotalAmount:C}</td>
                    </tr>
                </table>

                {(string.IsNullOrEmpty(invoice.ProjectName) ? "" : $"<p><strong>Proje:</strong> {invoice.ProjectName}</p>")}
                
                <p>Fatura detayları ekteki PDF dosyasında bulunmaktadır.</p>
                
                {(!string.IsNullOrEmpty(invoice.Notes) ? $"<p><strong>Notlar:</strong><br>{invoice.Notes}</p>" : "")}
                
                <p>Ödemelerinizi zamanında yapmanız rica olunur.</p>
                
                <br>
                <p>Saygılarımızla,<br>
                FreelanceFlow Sistemi</p>
            </body>
            </html>";

        var attachmentName = $"Fatura_{invoice.InvoiceNumber}.pdf";
        
        return await SendEmailAsync(to, subject, body, pdfAttachment, attachmentName);
    }
}