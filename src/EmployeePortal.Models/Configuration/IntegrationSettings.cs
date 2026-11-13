namespace EmployeePortal.Models.Configuration;

class IntegrationSettings
{
    public const string SectionName = "IntegrationSettings";

    public string DocumentStorageBasePath { get; set; } = "App_Data/EmployeeDocuments";
    public string SmtpHost { get; set; } = "smtp.dummy-hr-portal.local";
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsername { get; set; } = "hr-notifications@dummy-hr-portal.local";
    public string SmtpPassword { get; set; } = string.Empty;
    public string PayrollBankGatewayUrl { get; set; }
}
