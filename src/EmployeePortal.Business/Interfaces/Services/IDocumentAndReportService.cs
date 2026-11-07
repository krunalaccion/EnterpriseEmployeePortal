using EmployeePortal.Models.DTOs;

namespace EmployeePortal.Business.Interfaces.Services;

public interface IDocumentAndReportService
{
    Task<IReadOnlyList<EmployeeDocumentDto>> GetDocumentsAsync(int? employeeId = null, CancellationToken cancellationToken = default);
    Task<EmployeeDocumentDto?> GetDocumentMetadataByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeDocumentDto> UploadDocumentAsync(UploadDocumentRequestDto dto, string uploadedByUsername, CancellationToken cancellationToken = default);
    Task<byte[]> DownloadDocumentUnsanitizedAsync(string fileName, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReportTemplateDto>> GetReportTemplatesAsync(CancellationToken cancellationToken = default);
    Task<WorkforceAnalyticsReportDto> RunDynamicLinqReportAsync(DynamicReportQueryDto query, CancellationToken cancellationToken = default);
    Task<WorkforceAnalyticsReportDto> RunRawSqlReportAsync(string rawWhereClause, CancellationToken cancellationToken = default);
}

