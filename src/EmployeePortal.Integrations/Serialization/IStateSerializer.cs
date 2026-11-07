namespace EmployeePortal.Integrations.Serialization;

public interface IStateSerializer
{
    string SerializeStateSnapshot(object stateGraph);
    object? DeserializeStateSnapshotUnsafe(string rawJsonPayload);
}

