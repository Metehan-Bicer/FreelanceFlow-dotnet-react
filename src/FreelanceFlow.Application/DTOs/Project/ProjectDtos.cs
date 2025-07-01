using FreelanceFlow.Domain.Enums;

namespace FreelanceFlow.Application.DTOs.Project;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? DeadlineDate { get; set; }
    public decimal Budget { get; set; }
    public ProjectStatus Status { get; set; }
    public Priority Priority { get; set; }
    public int ProgressPercentage { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? DeadlineDate { get; set; }
    public decimal Budget { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
}

public class UpdateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? DeadlineDate { get; set; }
    public decimal Budget { get; set; }
    public Priority Priority { get; set; }
    public int ProgressPercentage { get; set; }
}

public class UpdateProjectStatusDto
{
    public ProjectStatus Status { get; set; }
}

public class UpdateProjectActiveStatusDto
{
    public bool IsActive { get; set; }
}