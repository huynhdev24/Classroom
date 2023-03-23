using System.ComponentModel.DataAnnotations;

namespace Classroom.Models.Catalog.Submissions;

public class SubmissionUpdateMarkRequest
{
    public int SubmissionID { set; get; }

    [Display(Name = "Điểm")]
    public float Mark { set; get; }

    [Display(Name = "Ghi chú")]
    public string? Note { set; get; }
}
