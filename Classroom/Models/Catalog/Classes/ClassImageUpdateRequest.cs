using System.ComponentModel.DataAnnotations;

namespace Classroom.Models.Catalog.Classes;

public class ClassImageUpdateRequest
{
    [Display(Name = "Hình ảnh")]
    public IFormFile? ThumbnailImage { get; set; }
}

