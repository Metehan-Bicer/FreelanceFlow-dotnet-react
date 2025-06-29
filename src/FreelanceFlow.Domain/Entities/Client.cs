using FreelanceFlow.Domain.Enums;

namespace FreelanceFlow.Domain.Entities;

public class Client : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public ClientStatus Status { get; set; } = ClientStatus.Active;
    public string? Notes { get; set; }
    
    // Navigation Properties
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}