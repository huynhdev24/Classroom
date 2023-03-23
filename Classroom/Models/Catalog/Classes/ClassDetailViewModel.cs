using System.ComponentModel.DataAnnotations;
using Classroom.Data.Enums;

namespace Classroom.Models.Catalog.Classes;

public class ClassDetailViewModel
{

    [Display(Name = "Mã lớp học")]
    public string? ClassID { set; get; }

    [Display(Name = "Tên lớp học")]
    public string? ClassName { set; get; }

    public string? UserID { set; get; }

    [Display(Name = "Tài khoản")]
    public string? UserName { set; get; }

    [Display(Name = "Họ")]
    public string? FirstName { set; get; }

    [Display(Name = "Tên")]
    public string? LastName { set; get; }

    [Display(Name = "Ảnh đại diện")]
    public string? Avatar { get; set; }

    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Display(Name = "Ngày sinh")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime Dob { get; set; }

    public Teacher IsTeacher { set; get; }
}

