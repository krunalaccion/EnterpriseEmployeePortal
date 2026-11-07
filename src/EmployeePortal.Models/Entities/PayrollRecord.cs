namespace EmployeePortal.Models.Entities;

public class PayrollRecord : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal FederalTaxWithheld { get; set; }
    public decimal StateTaxWithheld { get; set; }
    public decimal BenefitsDeduction { get; set; }
    public decimal BonusAmount { get; set; }
    public decimal NetPay { get; set; }
    public string PaymentStatus { get; set; } = "Processed";
    public string DirectDepositReference { get; set; } = string.Empty;
    public DateTime ProcessedDateUtc { get; set; } = DateTime.UtcNow;

    public Employee? Employee { get; set; }
}

