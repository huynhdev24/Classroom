using System.ComponentModel.DataAnnotations;

namespace Classroom.Models.System.Users;

public class UserDeleteRequest
{
    public string? Id { get; set; }
    [Display(Name = "Tên")]
    public string? FirstName { get; set; }

    [Display(Name = "Họ")]
    public string? LastName { get; set; }

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateTime Dob { get; set; }

    [Display(Name = "Địa chỉ email")]
    public string? Email { get; set; }

    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }
}