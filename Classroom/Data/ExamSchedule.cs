namespace Classroom.Data;

public class ExamSchedule
{
    public int ExamScheduleID { set; get; }
    public int ClassID { set; get; }
    public Class? Class { set; get; }
    public string? ExamScheduleName { set; get; }
    public DateTime DateTimeCreated { set; get; }
    public DateTime ExamDateTime { set; get; }
    public DateTime Deadline { set; get; }
    public string? Description { set; get; }
    public int ExamTime { set; get; }
    public List<StudentExam>? StudentExams { set; get; }
    public List<Question>? Questions { set; get; }
}

