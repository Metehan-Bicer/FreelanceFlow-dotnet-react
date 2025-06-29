using FreelanceFlow.Core.Common;

namespace FreelanceFlow.Application.Services.Interfaces;

public interface IDashboardService
{
    Task<Result<DashboardStatsDto>> GetDashboardStatsAsync();
    Task<Result<IEnumerable<RecentActivityDto>>> GetRecentActivitiesAsync(int count = 10);
    Task<Result<MonthlyRevenueDto>> GetMonthlyRevenueAsync();
    Task<Result<ProjectStatusStatsDto>> GetProjectStatusStatsAsync();
}

public class DashboardStatsDto
{
    public int TotalClients { get; set; }
    public int ActiveProjects { get; set; }
    public int PendingInvoices { get; set; }
    public int OverdueInvoices { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal PendingPayments { get; set; }
}

public class RecentActivityDto
{
    public string Type { get; set; } = string.Empty; // "project", "invoice", "client"
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class MonthlyRevenueDto
{
    public List<MonthlyRevenueItem> Data { get; set; } = new();
}

public class MonthlyRevenueItem
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int InvoiceCount { get; set; }
}

public class ProjectStatusStatsDto
{
    public int Planning { get; set; }
    public int InProgress { get; set; }
    public int OnHold { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
}