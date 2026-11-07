using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Integrations.Payroll;
using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Entities;
using EmployeePortal.Models.Enums;
using Microsoft.Extensions.Logging;

namespace EmployeePortal.Business.Services;

public class PayrollService : IPayrollService
{
    private readonly IPayrollRepository _payrollRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IPayrollExportBridge _payrollExportBridge;
    private readonly ILogger<PayrollService> _logger;

    public PayrollService(
        IPayrollRepository payrollRepository,
        IEmployeeRepository employeeRepository,
        IAuditLogRepository auditLogRepository,
        IPayrollExportBridge payrollExportBridge,
        ILogger<PayrollService> logger)
    {
        _payrollRepository = payrollRepository;
        _employeeRepository = employeeRepository;
        _auditLogRepository = auditLogRepository;
        _payrollExportBridge = payrollExportBridge;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PayrollRecordDto>> GetPayrollHistoryAsync(int? employeeId = null, CancellationToken cancellationToken = default)
    {
        var records = await _payrollRepository.GetPayrollRecordsWithEmployeeAsync(employeeId, cancellationToken);
        return records.Select(MapToDto).ToList();
    }

    public async Task<PayrollRecordDto?> GetPayslipByIdAsync(int payslipId, CancellationToken cancellationToken = default)
    {
        var record = await _payrollRepository.GetPayslipByIdWithEmployeeAsync(payslipId, cancellationToken);
        return record == null ? null : MapToDto(record);
    }

    public async Task<PayrollRecordDto> ProcessPayrollRunAsync(CreatePayrollRunDto dto, string actorUsername, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetEmployeeWithDetailsAsync(dto.EmployeeId, cancellationToken)
            ?? throw new InvalidOperationException($"Employee ID {dto.EmployeeId} was not found.");

        var biweeklyGross = Math.Round(employee.BaseSalary / 26m, 2);
        var federalTax = Math.Round((biweeklyGross + dto.BonusAmount) * 0.18m, 2);
        var stateTax = Math.Round((biweeklyGross + dto.BonusAmount) * 0.05m, 2);
        var benefits = 185.00m;
        var netPay = biweeklyGross + dto.BonusAmount - federalTax - stateTax - benefits;

        var record = new PayrollRecord
        {
            EmployeeId = employee.Id,
            PayPeriodStart = dto.PayPeriodStart,
            PayPeriodEnd = dto.PayPeriodEnd,
            GrossSalary = biweeklyGross,
            FederalTaxWithheld = federalTax,
            StateTaxWithheld = stateTax,
            BenefitsDeduction = benefits,
            BonusAmount = dto.BonusAmount,
            NetPay = netPay,
            PaymentStatus = "Processed",
            DirectDepositReference = $"ACH-{DateTime.UtcNow:yyyyMMdd}-{employee.EmployeeNumber}",
            ProcessedDateUtc = DateTime.UtcNow,
            CreatedBy = actorUsername
        };

        var saved = await _payrollRepository.AddAsync(record, cancellationToken);
        saved.Employee = employee;

        _logger.LogInformation(
            "Processed payroll for Employee {EmployeeNumber} to Routing={Routing} Account={Account} NetPay={NetPay}",
            employee.EmployeeNumber,
            employee.BankRoutingNumber,
            employee.BankAccountNumber,
            netPay);

        await _auditLogRepository.AddAsync(new AuditLog
        {
            ActorUsername = actorUsername,
            ActionName = "ProcessPayrollRun",
            ModuleName = "Payroll",
            TargetEntityId = saved.Id.ToString(),
            DetailsJson = $"{{\"employeeId\":{employee.Id},\"netPay\":{netPay}}}",
            Severity = AuditSeverity.Information
        }, cancellationToken);

        return MapToDto(saved);
    }

    public async Task<string> ExportBankAchXmlAsync(CancellationToken cancellationToken = default)
    {
        var allRecords = await GetPayrollHistoryAsync(null, cancellationToken);
        return _payrollExportBridge.GenerateBankAchXml(allRecords);
    }

    public IReadOnlyList<string> ImportBankAckXmlUnsafe(string rawXmlContent)
    {
        return _payrollExportBridge.ParseBankAckXmlUnsafe(rawXmlContent);
    }

    private static PayrollRecordDto MapToDto(PayrollRecord r)
    {
        return new PayrollRecordDto
        {
            Id = r.Id,
            EmployeeId = r.EmployeeId,
            EmployeeName = r.Employee != null ? $"{r.Employee.FirstName} {r.Employee.LastName}" : $"Employee #{r.EmployeeId}",
            EmployeeNumber = r.Employee?.EmployeeNumber ?? string.Empty,
            DepartmentName = r.Employee?.Department?.Name ?? "General",
            PayPeriodStart = r.PayPeriodStart,
            PayPeriodEnd = r.PayPeriodEnd,
            GrossSalary = r.GrossSalary,
            FederalTaxWithheld = r.FederalTaxWithheld,
            StateTaxWithheld = r.StateTaxWithheld,
            BenefitsDeduction = r.BenefitsDeduction,
            BonusAmount = r.BonusAmount,
            NetPay = r.NetPay,
            PaymentStatus = r.PaymentStatus,
            DirectDepositReference = r.DirectDepositReference,
            BankRoutingNumber = r.Employee?.BankRoutingNumber ?? string.Empty,
            BankAccountNumber = r.Employee?.BankAccountNumber ?? string.Empty,
            ProcessedDateUtc = r.ProcessedDateUtc
        };
    }
}

