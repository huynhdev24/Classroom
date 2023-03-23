namespace Classroom.Models.Catalog.Notifications;

public class NotificationImageViewModel
{
    public int? NotificationID { set; get; }

    public int? ImageID { set; get; }

    public string? ImagePath { get; set; }

    public bool? IsDefault { get; set; }
}

