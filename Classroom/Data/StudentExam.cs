namespace Classroom.Data;
public class StudentExam
{
    public int StudentExamID { set; get; }
    public int ExamScheduleID { set; get; }
    public ExamSchedule? ExamSchedule { set; get; }
    public string? StudentID { set; get; }
    public ApplicationUser? Student { set; get; }
    public float Mark { set; get; }
    public string? Note { set; get; }
    public DateTime StudentExamDateTime { set; get; }
    public DateTime SubmissionDateTime { set; get; }
}

