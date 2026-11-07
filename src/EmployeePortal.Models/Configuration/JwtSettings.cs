namespace EmployeePortal.Models.Configuration;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Issuer { get; set; } = "EnterpriseEmployeePortal.API";
    public string Audience { get; set; } = "EnterpriseEmployeePortal.Web";
    public string SigningKey { get; set; } = string.Empty;
    public int TokenExpiryMinutes { get; set; } = 120;
}

