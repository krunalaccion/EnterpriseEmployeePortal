using EmployeePortal.Models.Configuration;
using Microsoft.Extensions.Options;

namespace EmployeePortal.Integrations.Storage;

public class LocalFileStorageProvider : IDocumentStorageProvider
{
    private readonly string _baseDirectory;

    public LocalFileStorageProvider(IOptions<IntegrationSettings> options)
    {
        var configuredPath = options.Value.DocumentStorageBasePath;
        _baseDirectory = Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(AppContext.BaseDirectory, configuredPath);

        Directory.CreateDirectory(_baseDirectory);
    }

    public async Task<string> SaveDocumentAsync(string fileName, byte[] content, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_baseDirectory);
        var targetFilePath = Path.Combine(_baseDirectory, fileName);
        var targetDir = Path.GetDirectoryName(targetFilePath);
        if (!string.IsNullOrEmpty(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }

        await File.WriteAllBytesAsync(targetFilePath, content, cancellationToken);
        return fileName;
    }

    public async Task<byte[]> ReadDocumentSafeAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var safeFileName = Path.GetFileName(relativePath);
        var fullPath = Path.GetFullPath(Path.Combine(_baseDirectory, safeFileName));
        var canonicalBase = Path.GetFullPath(_baseDirectory);

        if (!fullPath.StartsWith(canonicalBase, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Resolved file path escapes storage root.");
        }

        if (!File.Exists(fullPath))
        {
            return Array.Empty<byte>();
        }

        return await File.ReadAllBytesAsync(fullPath, cancellationToken);
    }

    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-PATH-01 | CWE-22: Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal')
    // WARNING: INTENTIONALLY VULNERABLE TEST CASE FOR SECURESCANAI BENCHMARKING ONLY.
    // DO NOT USE IN PRODUCTION. Joins raw user-supplied fileName without canonical path validation.
    // =========================================================================
    public async Task<byte[]> ReadDocumentUnsanitizedAsync(string userSuppliedFileName, CancellationToken cancellationToken = default)
    {
        var combinedPath = Path.Combine(_baseDirectory, userSuppliedFileName);
        if (!File.Exists(combinedPath))
        {
            return Array.Empty<byte>();
        }

        return await File.ReadAllBytesAsync(combinedPath, cancellationToken);
    }
}

