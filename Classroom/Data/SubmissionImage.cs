namespace Classroom.Data;

/// <summary>
/// SubmissionImage
/// </summary>
public class SubmissionImage
{
    public int ImageID { set; get; }
    public int SubmissionID { set; get; }
    public Submission? Submission { set; get; }
    public string? ImagePath { set; get; }
    public long ImageFileSize { set; get; }
}