namespace EmployeePortal.Models.DTOs;

public class PayrollRecordDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal FederalTaxWithheld { get; set; }
    public decimal StateTaxWithheld { get; set; }
    public decimal BenefitsDeduction { get; set; }
    public decimal BonusAmount { get; set; }
    public decimal NetPay { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string DirectDepositReference { get; set; } = string.Empty;
    public string BankRoutingNumber { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public DateTime ProcessedDateUtc { get; set; }
}

public class CreatePayrollRunDto
{
    public int EmployeeId { get; set; }
    public DateTime PayPeriodStart { get; set; } = DateTime.UtcNow.AddDays(-14).Date;
    public DateTime PayPeriodEnd { get; set; } = DateTime.UtcNow.Date;
    public decimal BonusAmount { get; set; }
}

public class PayrollXmlImportDto
{
    public string XmlContent { get; set; } = string.Empty;
}

