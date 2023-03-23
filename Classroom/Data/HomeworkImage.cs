namespace Classroom.Data;

public class HomeworkImage
{
    public int ImageID { set; get; }
    public int HomeworkID { set; get; }
    public Homework? Homework { set; get; }
    public string? ImagePath { set; get; }
    public long ImageFileSize { set; get; }
}