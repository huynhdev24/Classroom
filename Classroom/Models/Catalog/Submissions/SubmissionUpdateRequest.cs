using System.ComponentModel.DataAnnotations;
using Classroom.Data;

namespace Classroom.Models.Catalog.Submissions;

public class SubmissionUpdateRequest
{
    public int SubmissionID { set; get; }
    [Display(Name = "Bài làm")]
    public string? Description { set; get; }
    public int ID { set; get; }
    [Display(Name = "Mã lớp học")]
    public string? ClassID { set; get; }

    [Display(Name = "Tên lớp học")]
    public string? ClassName { set; get; }
    public int HomeworkID { set; get; }
    [Display(Name = "Chọn tệp")]
    public List<IFormFile>? ThumbnailImages { get; set; }
    public ICollection<SubmissionImage>? SubmissionImages { get; set; }
}
