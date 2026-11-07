namespace EmployeePortal.Models.DTOs;

public class SystemSettingDto
{
    public int Id { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSensitive { get; set; }
}

public class UpdateSystemSettingDto
{
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string GroupName { get; set; } = "General";
    public string Description { get; set; } = string.Empty;
    public bool IsSensitive { get; set; }
}

public class StateSnapshotPayloadDto
{
    public string SerializedJsonState { get; set; } = string.Empty;
}

