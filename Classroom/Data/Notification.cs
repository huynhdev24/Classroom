namespace Classroom.Data;

/// <summary>
/// Notification
/// </summary>
public class Notification
{
    public int NotificationID { set; get; }
    public int ClassID { set; get; }
    public Class? Class { set; get; }
    public string? Title { set; get; }
    public string? Image { set; get; }
    public string? Content { set; get; }
    public DateTime DateTimeCreated { set; get; }
    public List<Comment>? Comments { set; get; }
    public List<NotificationImage>? NotificationImages { set; get; }
}