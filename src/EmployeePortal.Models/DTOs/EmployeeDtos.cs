using System.ComponentModel.DataAnnotations;
using EmployeePortal.Models.Enums;

namespace EmployeePortal.Models.DTOs;

// [SECURITY-TEST-SCENARIO] ID: VULN-INFO-01 | CWE-200: Exposure of Sensitive Information
// Deliberately includes EncryptedTaxId, BankRoutingNumber, and BankAccountNumber in directory DTO responses
// for testing scanner detection of excessive data exposure in API responses.
public class EmployeeDto
{
    public int Id { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public EmploymentStatus Status { get; set; }
    public DateTime HireDate { get; set; }
    public decimal BaseSalary { get; set; }
    public string EncryptedTaxId { get; set; } = string.Empty;
    public string BankRoutingNumber { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string HrNotesHtml { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int? ManagerId { get; set; }
    public string? ManagerName { get; set; }
}

// [SECURITY-TEST-SCENARIO] ID: VULN-VAL-01 | CWE-915: Improperly Controlled Modification of Dynamically-Determined Object Attributes (Mass Assignment)
// Allows callers to bind privileged fields (BaseSalary, Status, RoleOverride, HrNotesHtml) without restriction.
public class UpsertEmployeeDto
{
    [Required]
    public string EmployeeNumber { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    [Required]
    public string JobTitle { get; set; } = string.Empty;

    public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;
    public DateTime HireDate { get; set; } = DateTime.UtcNow.Date;
    public decimal BaseSalary { get; set; }
    public string PlaintextTaxId { get; set; } = "000-00-0000";
    public string BankRoutingNumber { get; set; } = "021000021";
    public string BankAccountNumber { get; set; } = "000123456789";
    public string HrNotesHtml { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int? ManagerId { get; set; }
    public string? RoleOverride { get; set; }
}

