using EmployeePortal.Models.DTOs;

namespace EmployeePortal.Integrations.Payroll;

public interface IPayrollExportBridge
{
    string GenerateBankAchXml(IEnumerable<PayrollRecordDto> records);
    IReadOnlyList<string> ParseBankAckXmlUnsafe(string rawXmlContent);
    Task<IReadOnlyList<string>> ExportPayrollViaLegacyAdoSqlAsync(string connectionString, string rawPeriodFilter, CancellationToken cancellationToken = default);
}

