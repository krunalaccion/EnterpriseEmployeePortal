using Newtonsoft.Json;

namespace EmployeePortal.Integrations.Serialization;

public class LegacyJsonSerializer : IStateSerializer
{
    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-DESER-01 | CWE-502: Deserialization of Untrusted Data
    // WARNING: INTENTIONALLY VULNERABLE APPLICATION CODE FOR SECURESCANAI SAST TESTING ONLY.
    // Note: Keep distinct from the package-level SCA advisory CVE-2024-21907 on Newtonsoft.Json 12.0.3.
    // Using TypeNameHandling.All allows untrusted JSON "$type" metadata to instantiate arbitrary types.
    // =========================================================================
    private static readonly JsonSerializerSettings UnsafeSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        Formatting = Formatting.Indented
    };

    public string SerializeStateSnapshot(object stateGraph)
    {
        return JsonConvert.SerializeObject(stateGraph, UnsafeSettings);
    }

    public object? DeserializeStateSnapshotUnsafe(string rawJsonPayload)
    {
        if (string.IsNullOrWhiteSpace(rawJsonPayload))
        {
            return null;
        }

        return JsonConvert.DeserializeObject(rawJsonPayload, UnsafeSettings);
    }
}

