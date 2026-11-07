namespace EmployeePortal.Models.Entities;

public class ReportTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = "Workforce";
    public string Description { get; set; } = string.Empty;
    public string DefaultFilterExpression { get; set; } = string.Empty;
    public string DefaultSortColumn { get; set; } = "LastName";
    public bool IsPublic { get; set; } = true;
}

