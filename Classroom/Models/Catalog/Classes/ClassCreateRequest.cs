using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Classroom.Models.Catalog.Classes;

public class ClassCreateRequest
{
    [Display(Name = "UserName")]
    public string? UserName { set; get; }
    
    [Display(Name = "Tên lớp học")]
    public string? ClassName { set; get; }

    [Display(Name = "Chủ đề")]
    public string? Topic { set; get; }

    [Display(Name = "Phòng học")]
    public string? ClassRoom { set; get; }

    [Display(Name = "Mô tả")]
    public string? Description { set; get; }

    [Display(Name = "Học phí")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N1}")]
    public decimal Tuition { set; get; }

    [Required]
    [Display(Name = "Hình ảnh")]
    public IFormFile? ThumbnailImage { get; set; }
}

