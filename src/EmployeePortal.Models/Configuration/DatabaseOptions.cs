namespace EmployeePortal.Models.Configuration;

public class DatabaseOptions
{
    public const string SectionName = "DatabaseOptions";

    /// <summary>
    /// Supported values: "SqlServer" (primary) or "InMemory" (development/test fallback).
    /// </summary>
    public string Provider { get; set; } = "SqlServer";
    public string InMemoryDatabaseName { get; set; } = "EnterpriseEmployeePortalDb";
    public bool EnableAutoSeed { get; set; } = true;
}

