namespace EmployeePortal.Business.Security;

public interface ICryptoHelper
{
    string ComputeUnsaltedMd5Hash(string input);
    string ComputeUnsaltedSha1Hash(string input);
    string EncryptTaxIdWithStaticDes(string plaintextTaxId);
    string DecryptTaxIdWithStaticDes(string base64Ciphertext);
}

