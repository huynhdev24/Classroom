using Classroom.Data.Enums;

namespace Classroom.Data;

public class Submission
{
    public int SubmissionID { set; get; }
    public int HomeworkID { set; get; }
    public Homework? Homework { set; get; }
    public string? StudentID { set; get; }
    public ApplicationUser? Student { set; get; }
    public float Mark { set; get; }
    public string? Note { set; get; }
    public string? Description { set; get; }
    public DateTime SubmissionDateTime { set; get; }
    public DateTime DateTimeUpdated { set; get; }
    public List<SubmissionImage>? SubmissionImages { set; get; }
}

