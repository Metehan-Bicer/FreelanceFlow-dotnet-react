namespace FreelanceFlow.Core.Common;

public static class Constants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Freelancer = "Freelancer";
        public const string Client = "Client";
    }

    public static class Policies
    {
        public const string RequireFreelancerRole = "RequireFreelancerRole";
        public const string RequireAdminRole = "RequireAdminRole";
    }

    public static class FileExtensions
    {
        public const string Pdf = ".pdf";
        public const string Excel = ".xlsx";
    }

    public static class EmailTemplates
    {
        public const string InvoiceSent = "InvoiceSent";
        public const string PaymentReceived = "PaymentReceived";
        public const string ProjectCompleted = "ProjectCompleted";
    }
}