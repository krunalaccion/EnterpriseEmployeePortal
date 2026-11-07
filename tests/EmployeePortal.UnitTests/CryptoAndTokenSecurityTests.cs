using EmployeePortal.Business.Security;
using EmployeePortal.Models.Configuration;
using EmployeePortal.Models.DTOs;
using Microsoft.Extensions.Options;
using Xunit;

namespace EmployeePortal.UnitTests;

public class CryptoAndTokenSecurityTests
{
    [Fact]
    public void LegacyCryptoHelper_Md5AndDes_RoundTripsDummyTaxId()
    {
        var crypto = new LegacyCryptoHelper();
        var md5 = crypto.ComputeUnsaltedMd5Hash("Admin@123!");
        var sha1 = crypto.ComputeUnsaltedSha1Hash("Admin@123!");

        Assert.Equal(32, md5.Length);
        Assert.Equal(40, sha1.Length);

        const string dummyTaxId = "999-42-1234";
        var encrypted = crypto.EncryptTaxIdWithStaticDes(dummyTaxId);
        var decrypted = crypto.DecryptTaxIdWithStaticDes(encrypted);

        Assert.NotEqual(dummyTaxId, encrypted);
        Assert.Equal(dummyTaxId, decrypted);
    }

    [Fact]
    public void JwtTokenGenerator_WithEmptyConfiguredKey_UsesHardcodedFallbackKeyAndValidates()
    {
        var options = Options.Create(new JwtSettings { SigningKey = string.Empty });
        var generator = new JwtTokenGenerator(options);

        var (token, _) = generator.GenerateToken(new UserSessionDto
        {
            UserId = 1,
            EmployeeId = 1,
            Username = "admin",
            Email = "admin@dummy-hr-portal.local",
            Roles = new List<string> { "Admin" }
        });

        Assert.False(string.IsNullOrWhiteSpace(token));

        var principal = generator.ValidateToken(token);
        Assert.NotNull(principal);
        Assert.True(principal!.IsInRole("Admin"));
    }
}

