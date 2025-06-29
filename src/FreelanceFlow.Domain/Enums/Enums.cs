namespace FreelanceFlow.Domain.Enums;

public enum ClientStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3
}

public enum ProjectStatus
{
    Planning = 1,
    InProgress = 2,
    OnHold = 3,
    Completed = 4,
    Cancelled = 5
}

public enum TaskStatus
{
    Todo = 1,
    InProgress = 2,
    InReview = 3,
    Completed = 4,
    Cancelled = 5
}

public enum InvoiceStatus
{
    Draft = 1,
    Sent = 2,
    Paid = 3,
    Overdue = 4,
    Cancelled = 5
}

public enum PaymentStatus
{
    Pending = 1,
    Paid = 2,
    PartiallyPaid = 3,
    Overdue = 4,
    Cancelled = 5
}

public enum Priority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum UserRole
{
    User = 1,
    Admin = 2,
    SuperAdmin = 3
}