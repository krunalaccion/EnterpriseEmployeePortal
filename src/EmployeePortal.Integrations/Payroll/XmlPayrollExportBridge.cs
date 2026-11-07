using System.Text;
using System.Xml;
using EmployeePortal.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace EmployeePortal.Integrations.Payroll;

public class XmlPayrollExportBridge : IPayrollExportBridge
{
    public string GenerateBankAchXml(IEnumerable<PayrollRecordDto> records)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<PayrollBatch>");
        foreach (var rec in records)
        {
            sb.AppendLine($"  <Payment id=\"{rec.Id}\" employee=\"{rec.EmployeeNumber}\" netPay=\"{rec.NetPay:F2}\" routing=\"{rec.BankRoutingNumber}\" account=\"{rec.BankAccountNumber}\" />");
        }
        sb.AppendLine("</PayrollBatch>");
        return sb.ToString();
    }

    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-DESER-02 | CWE-611 / CWE-502: Improper Restriction of XML External Entity Reference
    // WARNING: INTENTIONALLY VULNERABLE TEST CASE FOR SECURESCANAI SAST TESTING ONLY.
    // Enables DtdProcessing.Parse and XmlUrlResolver on untrusted XML input.
    // =========================================================================
    public IReadOnlyList<string> ParseBankAckXmlUnsafe(string rawXmlContent)
    {
        var results = new List<string>();
        if (string.IsNullOrWhiteSpace(rawXmlContent))
        {
            return results;
        }

        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Parse,
            XmlResolver = new XmlUrlResolver()
        };

        using var stringReader = new StringReader(rawXmlContent);
        using var xmlReader = XmlReader.Create(stringReader, settings);
        var doc = new XmlDocument { XmlResolver = new XmlUrlResolver() };
        doc.Load(xmlReader);

        var nodes = doc.SelectNodes("//Payment");
        if (nodes != null)
        {
            foreach (XmlNode node in nodes)
            {
                results.Add(node.OuterXml);
            }
        }

        return results;
    }

    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-SQLI-02 | CWE-89: SQL Injection via ADO.NET SqlCommand
    // Also exercises the isolated Microsoft.Data.SqlClient 5.1.2 dependency (CVE-2024-0056 / GHSA-98g6-xh36-x2p7).
    // NOTE: Requires a live SQL Server connection string; cannot run against EF Core InMemory provider.
    // =========================================================================
    public async Task<IReadOnlyList<string>> ExportPayrollViaLegacyAdoSqlAsync(
        string connectionString,
        string rawPeriodFilter,
        CancellationToken cancellationToken = default)
    {
        var rows = new List<string>();
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var unsafeCommandText = $"SELECT Id, DirectDepositReference, NetPay FROM PayrollRecords WHERE PaymentStatus = '{rawPeriodFilter}'";
        await using var command = new SqlCommand(unsafeCommandText, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            rows.Add($"{reader.GetInt32(0)}|{reader.GetString(1)}|{reader.GetDecimal(2):F2}");
        }

        return rows;
    }
}

