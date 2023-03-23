using System.ComponentModel.DataAnnotations;
using Classroom.Data;

namespace Classroom.Models.Catalog.Homeworks;

public class HomeworkViewModel
{
    public int HomeworkID { set; get; }

    [Display(Name = "Mã lớp học")]
    public int ID { set; get; }

    [Display(Name = "Tên lớp học")]
    public int ClassID { set; get; }
    public string? ClassName { set; get; }

    [Display(Name = "Tên bài tập")]
    public string? HomeworkName { set; get; }

    [Display(Name = "Mã giáo viên")]
    public string? TeacherID { set; get; }

    [Display(Name = "Mô tả")]
    public string? Description { set; get; }

    public DateTime SubmissionDateTime { set; get; }

    [Display(Name = "Ngày tạo")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm dddd dd/MM/yyyy}")]
    public DateTime DateTimeCreated { set; get; }

    [Display(Name = "Hạn nộp bài")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm dddd dd/MM/yyyy}")]
    public DateTime Deadline { set; get; }
    public ICollection<HomeworkImage>? HomeworkImages { get; set; }
    public ICollection<Submission>? Submissions { get; set; }
    public Submission? MySubmission { get; set; }
}

