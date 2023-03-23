using System.ComponentModel.DataAnnotations;

namespace Classroom.Models.Catalog.ExamSchedules;

public class ExamSchedulesUpdateRequest
{
    public int ExamScheduleID { set; get; }

    [Display(Name = "Tên kì thi")]
    public string? ExamScheduleName { set; get; }

    [Display(Name = "Ngày thi")]
    public DateTime ExamDateTime { set; get; }

    [Display(Name = "Hạn làm bài")]
    public DateTime Deadline { set; get; }

    [Display(Name = "Thời gian thi")]
    public int ExamTime { set; get; }
    [Display(Name = "Mô tả")]
    public string? Description { set; get; }
}