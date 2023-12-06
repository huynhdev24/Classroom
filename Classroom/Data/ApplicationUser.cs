using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Classroom.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [Display(Name = "Họ")]
    public string? FirstName { get; set; }

    [Display(Name = "Tên")]
    public string? LastName { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime Dob { get; set; }

    public string? Avatar { get; set; }

    public decimal AccountBalance { get; set; }

    public ICollection<ClassDetail>? ClassDetails { get; set; }

    public ICollection<StudentExam>? StudentExams { get; set; }

    public ICollection<Submission>? Submissions { get; set; }

    public ICollection<Comment>? Comments { get; set; }

    public ICollection<Room>? Rooms { get; set; }

    public ICollection<Message>? Messages { get; set; }
}

/// <summary>
/// ApplicationRole
/// </summary>
public class ApplicationRole : IdentityRole
{

}