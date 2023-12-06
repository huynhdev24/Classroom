using System.ComponentModel.DataAnnotations;

namespace Classroom.Models.System.Users;

/// <summary>
/// UserViewModel
/// </summary>
public class UserViewModel
{
    public string? Id { get; set; }

    public string? Avatar { get; set; }

    [Display(Name = "Tên")]
    public string? FirstName { get; set; }

    [Display(Name = "Họ")]
    public string? LastName { get; set; }

    [Display(Name = "Họ tên")]
    public string? FullName { get; set; }

    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Tài khoản")]
    public string? UserName { get; set; }

    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Display(Name = "Ngày sinh")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime Dob { get; set; }

    [Display(Name = "Thiết bị")]
    public string? Device { get; set; }

    [Display(Name = "Phòng hiện tại")]
    public string? CurrentRoom { get; set; }
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N0} đ")]
    public decimal AccountBalance { get; set; }

    public string? ConnectionId { get; set; }

    public IList<string>? Roles { get; set; }
}