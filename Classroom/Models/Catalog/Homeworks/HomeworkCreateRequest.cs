using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Classroom.Models.Catalog.Homeworks;

public class HomeworkCreateRequest
{
    [Display(Name = "Mã lớp học")]
    public int ClassID { set; get; }

    [Display(Name = "Tên bài tập")]
    public string? HomeworkName { set; get; }

    [Display(Name = "Mô tả")]
    public string? Description { set; get; }
    [Display(Name = "Ngày làm bài")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime SubmissionDateTime { set; get; }

    [Display(Name = "Hạn nộp bài")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime Deadline { set; get; }
    [Display(Name = "Hình ảnh")]
    public List<IFormFile>? ThumbnailImages { get; set; }
}

