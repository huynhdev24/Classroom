using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Classroom.Models.Catalog.Notifications;

public class NotificationUpdateRequest
{
    public int NotificationID { set; get; }
    public int ID { set; get; }
    [Display(Name = "Tên lớp học")]
    public string? ClassName { set; get; }

    [Display(Name = "Tiêu đề")]
    public string? Title { get; set; }

    [Display(Name = "Nội dung")]
    public string? Content { get; set; }

    [Display(Name = "Hình ảnh")]
    public List<IFormFile>? ThumbnailImages { get; set; }
}
