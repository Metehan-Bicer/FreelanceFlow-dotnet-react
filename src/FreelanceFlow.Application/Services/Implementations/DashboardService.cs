using FreelanceFlow.Application.Services.Interfaces;
using FreelanceFlow.Core.Common;
using FreelanceFlow.Domain.Interfaces.Repositories;
using FreelanceFlow.Domain.Enums;

namespace FreelanceFlow.Application.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IClientRepository _clientRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IInvoiceRepository _invoiceRepository;

    public DashboardService(
        IClientRepository clientRepository,
        IProjectRepository projectRepository,
        IInvoiceRepository invoiceRepository)
    {
        _clientRepository = clientRepository;
        _projectRepository = projectRepository;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<DashboardStatsDto>> GetDashboardStatsAsync()
    {
        try
        {
            var totalClients = await _clientRepository.CountAsync();
            var activeProjects = (await _projectRepository.GetActiveProjectsAsync()).Count();
            var pendingInvoices = (await _invoiceRepository.GetByPaymentStatusAsync(PaymentStatus.Pending)).Count();
            var overdueInvoices = (await _invoiceRepository.GetOverdueInvoicesAsync()).Count();
            var totalRevenue = await _invoiceRepository.GetTotalRevenueAsync();
            var monthlyRevenue = await _invoiceRepository.GetMonthlyRevenueAsync(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
            var pendingPayments = await _invoiceRepository.GetPendingPaymentsAsync();

            var stats = new DashboardStatsDto
            {
                TotalClients = totalClients,
                ActiveProjects = activeProjects,
                PendingInvoices = pendingInvoices,
                OverdueInvoices = overdueInvoices,
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue,
                PendingPayments = pendingPayments
            };

            return Result<DashboardStatsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            return Result<DashboardStatsDto>.Failure($"Dashboard istatistikleri yüklenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<RecentActivityDto>>> GetRecentActivitiesAsync(int count = 10)
    {
        try
        {
            var activities = new List<RecentActivityDto>();

            // Recent projects
            var recentProjects = (await _projectRepository.GetAllWithClientsAsync())
                .OrderByDescending(p => p.CreatedAt)
                .Take(count / 2)
                .Select(p => new RecentActivityDto
                {
                    Type = "project",
                    Title = $"Yeni Proje: {p.Name}",
                    Description = $"Müşteri: {p.Client?.Name}",
                    Date = p.CreatedAt,
                    Status = p.Status.ToString()
                });

            activities.AddRange(recentProjects);

            // Recent invoices
            var recentInvoices = (await _invoiceRepository.GetAllWithDetailsAsync())
                .OrderByDescending(i => i.CreatedAt)
                .Take(count / 2)
                .Select(i => new RecentActivityDto
                {
                    Type = "invoice",
                    Title = $"Fatura: {i.InvoiceNumber}",
                    Description = $"Müşteri: {i.Client?.Name} - {i.TotalAmount:C}",
                    Date = i.CreatedAt,
                    Status = i.Status.ToString()
                });

            activities.AddRange(recentInvoices);

            var sortedActivities = activities
                .OrderByDescending(a => a.Date)
                .Take(count);

            return Result<IEnumerable<RecentActivityDto>>.Success(sortedActivities);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<RecentActivityDto>>.Failure($"Son aktiviteler yüklenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<MonthlyRevenueDto>> GetMonthlyRevenueAsync()
    {
        try
        {
            var data = new List<MonthlyRevenueItem>();
            var currentDate = DateTime.UtcNow;

            // Son 12 ayın verilerini al
            for (int i = 11; i >= 0; i--)
            {
                var date = currentDate.AddMonths(-i);
                var revenue = await _invoiceRepository.GetMonthlyRevenueAsync(date.Year, date.Month);
                var invoiceCount = (await _invoiceRepository.GetAllWithDetailsAsync())
                    .Where(inv => inv.PaidAt.HasValue && 
                                 inv.PaidAt.Value.Year == date.Year && 
                                 inv.PaidAt.Value.Month == date.Month)
                    .Count();

                data.Add(new MonthlyRevenueItem
                {
                    Month = date.ToString("yyyy-MM"),
                    Revenue = revenue,
                    InvoiceCount = invoiceCount
                });
            }

            var result = new MonthlyRevenueDto { Data = data };
            return Result<MonthlyRevenueDto>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<MonthlyRevenueDto>.Failure($"Aylık gelir verileri yüklenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Result<ProjectStatusStatsDto>> GetProjectStatusStatsAsync()
    {
        try
        {
            var allProjects = await _projectRepository.GetAllWithClientsAsync();

            var stats = new ProjectStatusStatsDto
            {
                Planning = allProjects.Count(p => p.Status == ProjectStatus.Planning),
                InProgress = allProjects.Count(p => p.Status == ProjectStatus.InProgress),
                OnHold = allProjects.Count(p => p.Status == ProjectStatus.OnHold),
                Completed = allProjects.Count(p => p.Status == ProjectStatus.Completed),
                Cancelled = allProjects.Count(p => p.Status == ProjectStatus.Cancelled)
            };

            return Result<ProjectStatusStatsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            return Result<ProjectStatusStatsDto>.Failure($"Proje durum istatistikleri yüklenirken hata oluştu: {ex.Message}");
        }
    }
}