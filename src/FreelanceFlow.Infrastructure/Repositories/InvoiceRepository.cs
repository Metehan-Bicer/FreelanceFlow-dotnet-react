using Microsoft.EntityFrameworkCore;
using FreelanceFlow.Domain.Entities;
using FreelanceFlow.Domain.Interfaces.Repositories;
using FreelanceFlow.Domain.Enums;
using FreelanceFlow.Persistence.Context;

namespace FreelanceFlow.Infrastructure.Repositories;

public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(FreelanceFlowDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Invoice>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(i => i.Client)
            .Include(i => i.Project)
            .Include(i => i.Items)
            .Where(i => !i.IsDeleted)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<Invoice?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(i => i.Client)
            .Include(i => i.Project)
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task<IEnumerable<Invoice>> GetByClientIdAsync(Guid clientId)
    {
        return await _dbSet
            .Include(i => i.Client)
            .Include(i => i.Project)
            .Include(i => i.Items)
            .Where(i => i.ClientId == clientId && !i.IsDeleted)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByProjectIdAsync(Guid projectId)
    {
        return await _dbSet
            .Include(i => i.Client)
            .Include(i => i.Project)
            .Include(i => i.Items)
            .Where(i => i.ProjectId == projectId && !i.IsDeleted)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status)
    {
        return await _dbSet
            .Include(i => i.Client)
            .Include(i => i.Project)
            .Where(i => i.Status == status && !i.IsDeleted)
            .OrderBy(i => i.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByPaymentStatusAsync(PaymentStatus paymentStatus)
    {
        return await _dbSet
            .Include(i => i.Client)
            .Include(i => i.Project)
            .Where(i => i.PaymentStatus == paymentStatus && !i.IsDeleted)
            .OrderBy(i => i.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Include(i => i.Client)
            .Include(i => i.Project)
            .Where(i => i.DueDate.Date < today && 
                       i.PaymentStatus != PaymentStatus.Paid && 
                       i.Status != InvoiceStatus.Cancelled && 
                       !i.IsDeleted)
            .OrderBy(i => i.DueDate)
            .ToListAsync();
    }

    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var month = DateTime.UtcNow.Month;
        
        var count = await _dbSet
            .Where(i => i.CreatedAt.Year == year && i.CreatedAt.Month == month && !i.IsDeleted)
            .CountAsync();

        return $"INV-{year:D4}{month:D2}-{(count + 1):D4}";
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        return await _dbSet
            .Where(i => i.PaymentStatus == PaymentStatus.Paid && !i.IsDeleted)
            .SumAsync(i => i.TotalAmount);
    }

    public async Task<decimal> GetMonthlyRevenueAsync(int year, int month)
    {
        return await _dbSet
            .Where(i => i.PaymentStatus == PaymentStatus.Paid && 
                       i.PaidAt.HasValue &&
                       i.PaidAt.Value.Year == year && 
                       i.PaidAt.Value.Month == month && 
                       !i.IsDeleted)
            .SumAsync(i => i.TotalAmount);
    }

    public async Task<decimal> GetPendingPaymentsAsync()
    {
        return await _dbSet
            .Where(i => i.PaymentStatus != PaymentStatus.Paid && 
                       i.Status != InvoiceStatus.Cancelled && 
                       !i.IsDeleted)
            .SumAsync(i => i.TotalAmount);
    }
}