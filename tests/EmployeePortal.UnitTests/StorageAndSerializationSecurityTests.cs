using System.Text;
using EmployeePortal.Integrations.Imaging;
using EmployeePortal.Integrations.Serialization;
using EmployeePortal.Integrations.Storage;
using EmployeePortal.Models.Configuration;
using EmployeePortal.Models.DTOs;
using Microsoft.Extensions.Options;
using Xunit;

namespace EmployeePortal.UnitTests;

public class StorageAndSerializationSecurityTests
{
    [Fact]
    public void LegacyJsonSerializer_WithTypeNameHandlingAll_RoundTripsTypedObjectGraph()
    {
        var serializer = new LegacyJsonSerializer();
        var payload = new SystemSettingDto
        {
            Id = 99,
            SettingKey = "Test.Key",
            SettingValue = "DummyVal"
        };

        var json = serializer.SerializeStateSnapshot(payload);
        Assert.Contains("$type", json);

        var restored = serializer.DeserializeStateSnapshotUnsafe(json);
        Assert.IsType<SystemSettingDto>(restored);
    }

    [Fact]
    public async Task LocalFileStorageProvider_SaveAndReadSafe_WorksWithinRoot()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"PortalStorageTest_{Guid.NewGuid():N}");
        try
        {
            var provider = new LocalFileStorageProvider(Options.Create(new IntegrationSettings
            {
                DocumentStorageBasePath = tempRoot
            }));

            var expectedBytes = Encoding.UTF8.GetBytes("Dummy confidential HR test file");
            await provider.SaveDocumentAsync("policy.txt", expectedBytes);

            var readBytes = await provider.ReadDocumentSafeAsync("policy.txt");
            Assert.Equal(expectedBytes, readBytes);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    [Fact]
    public void BadgeAvatarProcessor_ReturnsFormattedBadgeMetadata()
    {
        var processor = new BadgeAvatarProcessor();
        var metadata = processor.GetBadgeDimensionsMetadata(128, 128);
        Assert.Contains("BadgeBounds=128x128", metadata);
    }
}

