using FreelanceFlow.Domain.Enums;

namespace FreelanceFlow.Domain.Entities;

public class ProjectTask : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public FreelanceFlow.Domain.Enums.TaskStatus Status { get; set; } = FreelanceFlow.Domain.Enums.TaskStatus.Todo;
    public Priority Priority { get; set; } = Priority.Medium;
    public decimal EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    public string? Notes { get; set; }
    
    // Navigation Properties
    public Project Project { get; set; } = null!;
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}