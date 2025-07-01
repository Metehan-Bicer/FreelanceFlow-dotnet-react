using FreelanceFlow.Application.Services.Interfaces;
using System.Net.Mail;
using System.Net;

namespace FreelanceFlow.Infrastructure.Services;

public class EmailService : IEmailService
{
    public async Task<bool> SendEmailAsync(string to, string subject, string body, byte[]? attachment = null, string? attachmentName = null)
    {
        try
        {
            // Mock email sending - gerçek email gönderimi için SMTP ayarları gerekli
            Console.WriteLine($"Email sent to {to} with subject: {subject}");
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email sending failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendInvoiceEmailAsync(string to, string clientName, string invoiceNumber, byte[] pdfAttachment)
    {
        var subject = $"Fatura #{invoiceNumber} - {clientName}";
        
        var body = $@"
            <html>
            <body>
                <h2>Sayın {clientName},</h2>
                <p>Faturanız hazırlanmıştır. Detaylar aşağıdaki gibidir:</p>
                
                <table style='border-collapse: collapse; width: 100%;'>
                    <tr>
                        <td style='border: 1px solid #ddd; padding: 8px; font-weight: bold;'>Fatura No:</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{invoiceNumber}</td>
                    </tr>
                </table>
                
                <p>Fatura detayları ekteki PDF dosyasında bulunmaktadır.</p>
                
                <p>Ödemelerinizi zamanında yapmanız rica olunur.</p>
                
                <br>
                <p>Saygılarımızla,<br>
                FreelanceFlow</p>
            </body>
            </html>";

        return await SendEmailAsync(to, subject, body, pdfAttachment, $"Fatura_{invoiceNumber}.pdf");
    }
}