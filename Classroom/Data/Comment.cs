namespace Classroom.Data;

/// <summary>
/// Comment
/// </summary>
public class Comment
{
    public int CommentID { set; get; }
    public int NotificationID { set; get; }
    public Notification? Notification { set; get; }
    public string? UserID { set; get; }
    public ApplicationUser? AppUser { set; get; }
    public string? Content { set; get; }
    public DateTime DateTimeCreated { set; get; }
    public string? ImagePath { set; get; }
}

