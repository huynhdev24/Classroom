using System.ComponentModel.DataAnnotations;

namespace Classroom.Models.Catalog.Notifications;

public class NotificationCreateRequest
{
    public string? ReturnUrl { set; get; }

    [Display(Name = "Mã lớp học")]
    public int ClassID { set; get; }

    [Required]
    [Display(Name = "Tiêu đề")]
    public string? Title { get; set; }

    [Display(Name = "Nội dung")]
    public string? Content { get; set; }

    [Display(Name = "Hình ảnh")]
    public List<IFormFile>? ThumbnailImages { get; set; }
}

