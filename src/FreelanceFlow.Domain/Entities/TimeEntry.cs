namespace FreelanceFlow.Domain.Entities;

public class TimeEntry : BaseEntity
{
    public Guid ProjectId { get; set; }
    public Guid? TaskId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal Hours { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsBillable { get; set; } = true;
    public bool IsInvoiced { get; set; } = false;
    
    // Navigation Properties
    public Project Project { get; set; } = null!;
    public ProjectTask? Task { get; set; }
}