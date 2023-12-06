namespace Classroom.Data;

/// <summary>
/// Homework
/// </summary>
public class Homework
{
    public int HomeworkID { set; get; }
    public int ClassID { set; get; }
    public Class? Class { set; get; }
    public string? HomeworkName { set; get; }
    public string? Description { set; get; }
    public DateTime DateTimeCreated { set; get; }
    public DateTime SubmissionDateTime { set; get; }
    public DateTime Deadline { set; get; }
    public List<Submission>? Submissions { set; get; }
    public List<HomeworkImage>? HomeworkImages { set; get; }
}
