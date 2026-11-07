namespace EmployeePortal.Models.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string CostCenter { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal AnnualBudget { get; set; }
    public int? ManagerEmployeeId { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

