using System.ComponentModel.DataAnnotations;

namespace Classroom.Models.Catalog.Comments;

public class CommentViewModel
{
    public int CommentID { set; get; }
    public int NotificationID { set; get; }
    public string? UserID { set; get; }
    public string? UserName { set; get; }

    [Display(Name = "Ảnh đại diện")]
    public string? Avatar { set; get; }

    [Display(Name = "Họ tên")]
    public string? FullName { set; get; }

    [Display(Name = "Nội dung")]
    public string? Content { set; get; }

    [Display(Name = "Ngày tạo")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime DateTimeCreated { set; get; }
}

