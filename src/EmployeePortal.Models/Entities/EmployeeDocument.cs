using EmployeePortal.Models.Enums;

namespace EmployeePortal.Models.Entities;

public class EmployeeDocument : BaseEntity
{
    public int EmployeeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DocumentCategory Category { get; set; } = DocumentCategory.GeneralPolicy;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public long FileSizeBytes { get; set; }
    public string StorageRelativePath { get; set; } = string.Empty;
    public bool IsConfidential { get; set; }
    public string UploadedByUsername { get; set; } = "system";

    public Employee? Employee { get; set; }
}

