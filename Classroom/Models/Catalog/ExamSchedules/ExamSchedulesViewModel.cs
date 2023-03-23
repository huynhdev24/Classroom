using System.ComponentModel.DataAnnotations;
using Classroom.Data;

namespace Classroom.Models.Catalog.ExamSchedules;

public class ExamSchedulesViewModel
{
    public int ExamScheduleID { set; get; }

    [Display(Name = "Mã lớp học")]       
    public int? ClassID { set; get; }

    [Display(Name = "Tên lớp học")]
    public string? ClassName { get; set; }
    public string? TeacherID { get; set; }

    [Display(Name = "Tên kì thi")]
    public string? ExamScheduleName { set; get; }

    [Display(Name = "Ngày tạo")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm dddd dd/MM/yyyy}")]
    public DateTime DateTimeCreated { set; get; }

    [Display(Name = "Ngày thi")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm dddd dd/MM/yyyy}")]
    public DateTime ExamDateTime { set; get; }

    [Display(Name = "Hạn làm bài")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm dddd dd/MM/yyyy}")]
    public DateTime Deadline { set; get; }

    [Display(Name = "Thời gian thi")]
    public int ExamTime { set; get; }
    public string? Description { set; get; }
    public StudentExam? MyStudentExam { set; get; }

}