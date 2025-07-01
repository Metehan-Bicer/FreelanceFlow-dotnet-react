using FreelanceFlow.Application.DTOs.Invoice;
using FreelanceFlow.Domain.Entities;
using FreelanceFlow.Core.Common;

namespace FreelanceFlow.Application.Services.Interfaces;

public interface IPdfService
{
    Task<byte[]> GenerateInvoicePdfAsync(Invoice invoice);
}

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, byte[]? attachment = null, string? attachmentName = null);
    Task<bool> SendInvoiceEmailAsync(string to, string clientName, string invoiceNumber, byte[] pdfAttachment);
}