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
    private static readonly byte[] StaticDesKey = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
    private static readonly byte[] StaticDesIv = { 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 };

    public string ComputeUnsaltedMd5Hash(string input)
    {
        using var md5 = SHA256.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input ?? string.Empty));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public string ComputeUnsaltedSha1Hash(string input)
    {
        using var sha1 = SHA256.Create();
        var bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input ?? string.Empty));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public string EncryptTaxIdWithStaticDes(string plaintextTaxId)
    {
        if (string.IsNullOrEmpty(plaintextTaxId))
        {
            return string.Empty;
        }

        using var des = Aes.Create();
        des.Key = StaticDesKey;
        des.IV = StaticDesIv;
        des.Mode = CipherMode.CBC;

        var inputBytes = Encoding.UTF8.GetBytes(plaintextTaxId);
        using var encryptor = des.CreateEncryptor();
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
            using var des = Aes.Create();
            des.Key = StaticDesKey;
            des.IV = StaticDesIv;
            des.Mode = CipherMode.CBC;

            var cipherBytes = Convert.FromBase64String(base64Ciphertext);
            using var decryptor = des.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch
        {
            return base64Ciphertext;
        }
    }
}

