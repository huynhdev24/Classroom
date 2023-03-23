namespace Classroom.Models.Catalog.StudentExams
{
    public class StudentExamsViewModel
    {
        public int StudentExamID { set; get; }
        public int ExamScheduleID { set; get; }
        public string? ExamScheduleName { set; get; }
        public int ClassID { set; get; }
        public string? ClassName { set; get; }
        public string? StudentID { set; get; }
        public string? FirstName { set; get; }
        public string? LastName { set; get; }
        public float Mark { set; get; }
        public string? Note { set; get; }
        public DateTime DateTimeStudentExam { set; get; }
    }
}