namespace Classroom.Data;

public class Question
{
    public int QuestionID { set; get; }
    public int ExamScheduleID { set; get; }
    public ExamSchedule? ExamSchedule { set; get; }
    public string? QuestionString { set; get; }
    public float Point { set; get; }
    public string? Option1 { get; set; }
    public string? Option2 { get; set; }
    public string? Option3 { get; set; }
    public string? Option4 { get; set; }
    public int OptionCorrect { get; set; }
}

