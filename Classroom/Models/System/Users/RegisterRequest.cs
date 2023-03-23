using System.ComponentModel.DataAnnotations;

namespace Classroom.Models.System.Users;

public class RegisterRequest
{
    [Display(Name = "Họ")]
    public string? FirstName { get; set; }

    [Display(Name = "Tên")]
    public string? LastName { get; set; }

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateTime Dob { get; set; }

    [Display(Name = "Địa chỉ email")]
    public string? Email { get; set; }

    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Tài khoản")]
    public string? UserName { get; set; }

    [Display(Name = "Mật khẩu")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Xác nhận mật khẩu")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }
    public string? Avatar { get; set; }
}