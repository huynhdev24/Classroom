using System.ComponentModel.DataAnnotations;
using Classroom.Data.Enums;

namespace Classroom.Data;

/// <summary>
/// Contact
/// </summary>
public class Contact
{
    public int ContactID { set; get; }
    public string? CustomerName { set; get; }
    public string? Email { set; get; }
    public string? PhoneNumber { set; get; }
    [Required]
    public string? Message { set; get; }
    public DateTime DateTimeCreated { set; get; }
    public Status Status { set; get; }
}