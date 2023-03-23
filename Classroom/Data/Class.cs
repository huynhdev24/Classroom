using Classroom.Data.Enums;

namespace Classroom.Data;

public class Class
{
    public int ID { set; get; }
    public string? ClassID { set; get; }
    public string? ClassName { set; get; }
    public string? ImagePath { set; get; }
    public string? Topic { set; get; }
    public string? ClassRoom { set; get; }
    public string? Description { set; get; }
    public decimal Tuition { set; get; }
    public DateTime DateCreated { set; get; }
    public int ViewCount { set; get; }
    public Status Status { set; get; }
    public IsPublic isPublic { set; get; }
    public ICollection<ClassDetail>? ClassDetails { set; get; }
    public ICollection<Homework>? Homeworks { set; get; }
    public ICollection<Notification>? Notifications { set; get; }
    public ICollection<ExamSchedule>? ExamSchedules { set; get; }
}