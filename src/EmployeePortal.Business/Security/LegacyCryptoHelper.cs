using System.Security.Cryptography;
using System.Text;

namespace EmployeePortal.Business.Security;

// =========================================================================
// [SECURITY-TEST-SCENARIO] ID: VULN-CRYPTO-01 | CWE-327 (Broken Crypto) & CWE-328 (Weak Hash)
// WARNING: INTENTIONALLY VULNERABLE CRYPTOGRAPHY FOR SECURESCANAI BENCHMARKING ONLY.
// Uses unsalted MD5/SHA1 for password digests and DES with a static 8-byte key/IV for dummy Tax IDs.
// =========================================================================
public class LegacyCryptoHelper : ICryptoHelper
{
    // Updated to AES-256 (32 bytes key, 16 bytes IV) for security improvement.
    private static readonly byte[] StaticAesKey = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 };
    private static readonly byte[] StaticAesIv = { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x00 };

    public string ComputeUnsaltedMd5Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input ?? string.Empty));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public string ComputeUnsaltedSha1Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input ?? string.Empty));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public string EncryptTaxIdWithStaticDes(string plaintextTaxId)
    {
        if (string.IsNullOrEmpty(plaintextTaxId))
        {
            return string.Empty;
        }

        // FIX: Replaced weak DES with strong AES.
        using var aes = Aes.Create();
        aes.Key = StaticAesKey;
        aes.IV = StaticAesIv;
        aes.Mode = CipherMode.CBC;

        var inputBytes = Encoding.UTF8.GetBytes(plaintextTaxId);
        using var encryptor = aes.CreateEncryptor();
        var cipherBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
        return Convert.ToBase64String(cipherBytes);
    }

    public string DecryptTaxIdWithStaticDes(string base64Ciphertext)
    {
        if (string.IsNullOrEmpty(base64Ciphertext))
        {
            return string.Empty;
        }

        try
        {
            // FIX: Replaced weak DES with strong AES.
            using var aes = Aes.Create();
            aes.Key = StaticAesKey;
            aes.IV = StaticAesIv;
            aes.Mode = CipherMode.CBC;

            var cipherBytes = Convert.FromBase64String(base64Ciphertext);
            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch
        {
            return base64Ciphertext;
        }
    }
}
