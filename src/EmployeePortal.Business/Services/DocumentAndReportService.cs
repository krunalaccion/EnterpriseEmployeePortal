using AutoMapper;
using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Integrations.Storage;
using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Entities;

namespace EmployeePortal.Business.Services;

public class DocumentAndReportService : IDocumentAndReportService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IReportRepository _reportRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDocumentStorageProvider _storageProvider;
    private readonly IMapper _mapper;

    public DocumentAndReportService(
        IDocumentRepository documentRepository,
        IReportRepository reportRepository,
        IEmployeeRepository employeeRepository,
        IDocumentStorageProvider storageProvider,
        IMapper mapper)
    {
        _documentRepository = documentRepository;
        _reportRepository = reportRepository;
        _employeeRepository = employeeRepository;
        _storageProvider = storageProvider;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<EmployeeDocumentDto>> GetDocumentsAsync(int? employeeId = null, CancellationToken cancellationToken = default)
    {
        var docs = await _documentRepository.GetDocumentsWithEmployeeAsync(employeeId, cancellationToken);
        return docs.Select(MapDocument).ToList();
    }

    public async Task<EmployeeDocumentDto?> GetDocumentMetadataByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var doc = await _documentRepository.GetByIdAsync(id, cancellationToken);
        return doc == null ? null : MapDocument(doc);
    }

    public async Task<EmployeeDocumentDto> UploadDocumentAsync(UploadDocumentRequestDto dto, string uploadedByUsername, CancellationToken cancellationToken = default)
    {
        var rawBytes = string.IsNullOrWhiteSpace(dto.Base64Content)
            ? System.Text.Encoding.UTF8.GetBytes($"Dummy Document Content for {dto.Title}")
            : Convert.FromBase64String(dto.Base64Content);

        var storedRelativePath = await _storageProvider.SaveDocumentAsync(dto.FileName, rawBytes, cancellationToken);

        var entity = new EmployeeDocument
        {
            EmployeeId = dto.EmployeeId,
            Title = dto.Title,
            Category = dto.Category,
            FileName = dto.FileName,
            ContentType = dto.ContentType,
            FileSizeBytes = rawBytes.LongLength,
            StorageRelativePath = storedRelativePath,
            IsConfidential = dto.IsConfidential,
            UploadedByUsername = uploadedByUsername,
            CreatedBy = uploadedByUsername
        };

        var saved = await _documentRepository.AddAsync(entity, cancellationToken);
        var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId, cancellationToken);
        saved.Employee = employee;

        return MapDocument(saved);
    }

    public async Task<byte[]> DownloadDocumentUnsanitizedAsync(string fileName, CancellationToken cancellationToken = default)
    {
        return await _storageProvider.ReadDocumentUnsanitizedAsync(fileName, cancellationToken);
    }

    public async Task<IReadOnlyList<ReportTemplateDto>> GetReportTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var templates = await _reportRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ReportTemplateDto>>(templates);
    }

    public async Task<WorkforceAnalyticsReportDto> RunDynamicLinqReportAsync(DynamicReportQueryDto query, CancellationToken cancellationToken = default)
    {
        var employees = await _reportRepository.QueryEmployeesDynamicLinqAsync(
            query.DynamicWhereClause,
            query.OrderByExpression,
            cancellationToken);

        var mapped = _mapper.Map<List<EmployeeDto>>(employees);
        return new WorkforceAnalyticsReportDto
        {
            ReportTitle = string.IsNullOrWhiteSpace(query.DynamicWhereClause)
                ? "All Workforce Compensation Report"
                : $"Dynamic Filter Report ({query.DynamicWhereClause})",
            GeneratedAtUtc = DateTime.UtcNow,
            MatchedRowCount = mapped.Count,
            TotalCompensation = mapped.Sum(e => e.BaseSalary),
            AverageCompensation = mapped.Count > 0 ? Math.Round(mapped.Average(e => e.BaseSalary), 2) : 0m,
            MatchedEmployees = mapped
        };
    }

    public async Task<WorkforceAnalyticsReportDto> RunRawSqlReportAsync(string rawWhereClause, CancellationToken cancellationToken = default)
    {
        var employees = await _reportRepository.ExecuteCustomReportRawSqlAsync(rawWhereClause, cancellationToken);
        var mapped = _mapper.Map<List<EmployeeDto>>(employees);
        return new WorkforceAnalyticsReportDto
        {
            ReportTitle = $"Raw SQL Workforce Report ({rawWhereClause})",
            GeneratedAtUtc = DateTime.UtcNow,
            MatchedRowCount = mapped.Count,
            TotalCompensation = mapped.Sum(e => e.BaseSalary),
            AverageCompensation = mapped.Count > 0 ? Math.Round(mapped.Average(e => e.BaseSalary), 2) : 0m,
            MatchedEmployees = mapped
        };
    }

    private static EmployeeDocumentDto MapDocument(EmployeeDocument d)
    {
        return new EmployeeDocumentDto
        {
            Id = d.Id,
            EmployeeId = d.EmployeeId,
            EmployeeName = d.Employee != null ? $"{d.Employee.FirstName} {d.Employee.LastName}" : $"Employee #{d.EmployeeId}",
            Title = d.Title,
            Category = d.Category,
            FileName = d.FileName,
            ContentType = d.ContentType,
            FileSizeBytes = d.FileSizeBytes,
            StorageRelativePath = d.StorageRelativePath,
            IsConfidential = d.IsConfidential,
            UploadedByUsername = d.UploadedByUsername,
            CreatedAtUtc = d.CreatedAtUtc
        };
    }
}

