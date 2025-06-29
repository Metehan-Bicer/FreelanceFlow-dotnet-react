using FreelanceFlow.Application.DTOs.Invoice;
using FreelanceFlow.Core.Common;

namespace FreelanceFlow.Application.Services.Interfaces;

public interface IPdfService
{
    Task<Result<byte[]>> GenerateInvoicePdfAsync(InvoiceDto invoice);
}

public interface IEmailService
{
    Task<Result<string>> SendEmailAsync(string to, string subject, string body, byte[]? attachment = null, string? attachmentName = null);
    Task<Result<string>> SendInvoiceEmailAsync(string to, InvoiceDto invoice, byte[] pdfAttachment);
}