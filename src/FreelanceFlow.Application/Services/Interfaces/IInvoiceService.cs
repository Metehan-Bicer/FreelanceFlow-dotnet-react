using FreelanceFlow.Application.DTOs.Invoice;
using FreelanceFlow.Core.Common;
using FreelanceFlow.Domain.Enums;

namespace FreelanceFlow.Application.Services.Interfaces;

public interface IInvoiceService
{
    Task<Result<IEnumerable<InvoiceDto>>> GetAllAsync();
    Task<Result<InvoiceDto>> GetByIdAsync(Guid id);
    Task<Result<Guid>> CreateAsync(CreateInvoiceDto dto);
    Task<Result<InvoiceDto>> UpdateAsync(Guid id, UpdateInvoiceDto dto);
    Task<Result<string>> DeleteAsync(Guid id);
    Task<Result<string>> UpdateStatusAsync(Guid id, InvoiceStatus status);
    Task<Result<string>> UpdatePaymentStatusAsync(Guid id, PaymentStatus paymentStatus, DateTime? paidAt);
    Task<Result<string>> SendInvoiceByEmailAsync(Guid id);
    Task<Result<IEnumerable<InvoiceDto>>> GetByClientIdAsync(Guid clientId);
    Task<Result<IEnumerable<InvoiceDto>>> GetByProjectIdAsync(Guid projectId);
    Task<Result<IEnumerable<InvoiceDto>>> GetOverdueInvoicesAsync();
}