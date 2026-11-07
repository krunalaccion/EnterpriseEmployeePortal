using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.Models.DTOs;

public class DepartmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string CostCenter { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal AnnualBudget { get; set; }
    public int? ManagerEmployeeId { get; set; }
    public string? ManagerName { get; set; }
    public int EmployeeCount { get; set; }
    public bool IsActive { get; set; }
}

public class UpsertDepartmentDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;

    public string CostCenter { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal AnnualBudget { get; set; }
    public int? ManagerEmployeeId { get; set; }
    public bool IsActive { get; set; } = true;
}

