using FreelanceFlow.Domain.Enums;

namespace FreelanceFlow.Domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public Guid? ProjectId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; } = 0.18m; // %18 KDV
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; } = 0;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public DateTime? PaidAt { get; set; }
    public string? Notes { get; set; }
    public string? PdfFilePath { get; set; }
    
    // Navigation Properties
    public Client Client { get; set; } = null!;
    public Project? Project { get; set; }
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}