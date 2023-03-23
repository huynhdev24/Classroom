using System.ComponentModel.DataAnnotations;

namespace Classroom.Models.Catalog.Homeworks;

public class HomeworkUpdateRequest
{
    public int HomeworkID { set; get; }

    [Display(Name = "Tên bài tập")]
    public string? HomeworkName { set; get; }
    [Display(Name = "Mã lớp học")]
    public int ID { set; get; }

    [Display(Name = "Tên lớp học")]
    public string? ClassName { set; get; }

    [Display(Name = "Mô tả")]
    public string? Description { set; get; }

    [Display(Name = "Ngày nộp bài")]
    public DateTime SubmissionDateTime { set; get; }

    [Display(Name = "Hạn nộp bài")]
    public DateTime Deadline { set; get; }
    [Display(Name = "Hình ảnh")]
    public List<IFormFile>? ThumbnailImages { get; set; }
}
