namespace EmployeePortal.Integrations.Storage;

public interface IDocumentStorageProvider
{
    Task<string> SaveDocumentAsync(string fileName, byte[] content, CancellationToken cancellationToken = default);
    Task<byte[]> ReadDocumentSafeAsync(string relativePath, CancellationToken cancellationToken = default);

    // [SECURITY-TEST-SCENARIO] CWE-22: Path Traversal test contract
    Task<byte[]> ReadDocumentUnsanitizedAsync(string userSuppliedFileName, CancellationToken cancellationToken = default);
}

