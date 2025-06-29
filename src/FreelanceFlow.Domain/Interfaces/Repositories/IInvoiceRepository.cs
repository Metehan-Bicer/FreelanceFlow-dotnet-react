using FreelanceFlow.Domain.Entities;
using FreelanceFlow.Domain.Enums;

namespace FreelanceFlow.Domain.Interfaces.Repositories;

public interface IInvoiceRepository : IBaseRepository<Invoice>
{
    Task<IEnumerable<Invoice>> GetAllWithDetailsAsync();
    Task<Invoice?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Invoice>> GetByClientIdAsync(Guid clientId);
    Task<IEnumerable<Invoice>> GetByProjectIdAsync(Guid projectId);
    Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status);
    Task<IEnumerable<Invoice>> GetByPaymentStatusAsync(PaymentStatus paymentStatus);
    Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync();
    Task<string> GenerateInvoiceNumberAsync();
    Task<decimal> GetTotalRevenueAsync();
    Task<decimal> GetMonthlyRevenueAsync(int year, int month);
    Task<decimal> GetPendingPaymentsAsync();
}