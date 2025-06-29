using FreelanceFlow.Domain.Enums;

namespace FreelanceFlow.Domain.Entities;

public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? DeadlineDate { get; set; }
    public decimal Budget { get; set; }
    public decimal? ActualCost { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
    public Priority Priority { get; set; } = Priority.Medium;
    public string? Notes { get; set; }
    public int ProgressPercentage { get; set; } = 0;
    
    // Navigation Properties
    public Client Client { get; set; } = null!;
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}