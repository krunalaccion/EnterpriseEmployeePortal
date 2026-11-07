namespace EmployeePortal.Models.DTOs;

public class ReportTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DefaultFilterExpression { get; set; } = string.Empty;
    public string DefaultSortColumn { get; set; } = string.Empty;
}

public class DynamicReportQueryDto
{
    public string DynamicWhereClause { get; set; } = string.Empty;
    public string OrderByExpression { get; set; } = "LastName ascending";
    public string RawSqlFilter { get; set; } = string.Empty;
}

public class WorkforceAnalyticsReportDto
{
    public string ReportTitle { get; set; } = string.Empty;
    public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;
    public int MatchedRowCount { get; set; }
    public decimal TotalCompensation { get; set; }
    public decimal AverageCompensation { get; set; }
    public List<EmployeeDto> MatchedEmployees { get; set; } = new();
}

