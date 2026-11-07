namespace EmployeePortal.Models.Entities;

public class Notification : BaseEntity
{
    public int? RecipientUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MessageHtml { get; set; } = string.Empty;
    public string Category { get; set; } = "Announcement";
    public bool IsRead { get; set; }
    public bool IsBroadcast { get; set; }
}

