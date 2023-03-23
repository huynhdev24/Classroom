using System.ComponentModel.DataAnnotations;
using Classroom.Data;
using Classroom.Models.Catalog.Comments;

namespace Classroom.Models.Catalog.Notifications;

public class NotificationViewModel
{
    public int NotificationID { set; get; }

    [Display(Name = "Mã lớp học")]
    public string? ClassID { set; get; }
    public int? ID { set; get; }

    [Display(Name = "Tên lớp học")]
    public string? ClassName { set; get; }

    [Display(Name = "Tiêu đề")]
    public string? Title { get; set; }

    [Display(Name = "Nội dung")]
    public string? Content { get; set; }

    [Display(Name = "Ngày tạo")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime DateTimeCreated { set; get; }

    public ICollection<NotificationImage>? NotificationImages { get; set; }

    public ICollection<CommentViewModel>? Comments { get; set; }
}

