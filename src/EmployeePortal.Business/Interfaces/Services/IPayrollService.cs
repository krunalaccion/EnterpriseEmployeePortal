using EmployeePortal.Models.DTOs;

namespace EmployeePortal.Business.Interfaces.Services;

public interface IPayrollService
{
    Task<IReadOnlyList<PayrollRecordDto>> GetPayrollHistoryAsync(int? employeeId = null, CancellationToken cancellationToken = default);
    Task<PayrollRecordDto?> GetPayslipByIdAsync(int payslipId, CancellationToken cancellationToken = default);
    Task<PayrollRecordDto> ProcessPayrollRunAsync(CreatePayrollRunDto dto, string actorUsername, CancellationToken cancellationToken = default);
    Task<string> ExportBankAchXmlAsync(CancellationToken cancellationToken = default);
    IReadOnlyList<string> ImportBankAckXmlUnsafe(string rawXmlContent);
}

