using EmployeePortal.Models.Enums;

namespace EmployeePortal.Models.DTOs;

public class EmployeeDocumentDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DocumentCategory Category { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string StorageRelativePath { get; set; } = string.Empty;
    public bool IsConfidential { get; set; }
    public string UploadedByUsername { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}

public class UploadDocumentRequestDto
{
    public int EmployeeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DocumentCategory Category { get; set; } = DocumentCategory.GeneralPolicy;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public string Base64Content { get; set; } = string.Empty;
    public bool IsConfidential { get; set; }
}

