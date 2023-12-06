namespace Classroom.Data;

/// <summary>
/// HomeworkImage
/// </summary>
public class HomeworkImage
{
    public int ImageID { set; get; }
    public int HomeworkID { set; get; }
    public Homework? Homework { set; get; }
    public string? ImagePath { set; get; }
    public long ImageFileSize { set; get; }
}