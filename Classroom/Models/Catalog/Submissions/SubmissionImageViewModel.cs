namespace Classroom.Models.Catalog.Submissions;

public class SubmissionImageViewModel
{
    public int? SubmissionID { set; get; }

    public int? ImageID { set; get; }

    public string? ImagePath { get; set; }

    public bool? IsDefault { get; set; }
}

