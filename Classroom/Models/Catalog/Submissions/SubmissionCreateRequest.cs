using System.ComponentModel.DataAnnotations;

namespace Classroom.Models.Catalog.Submissions;

public class SubmissionCreateRequest
{
    public int HomeworkID { set; get; }

    public string? StudentID { set; get; }

    [Display(Name = "Bài làm")]
    public string? Description { set; get; }
    public List<IFormFile>? ThumbnailImages { get; set; }
    public string? ReturnUrl { get; set; }
}

