namespace Classroom.Models.System.Users;

public class ResetPasswordRequest
{
    public string? UserName { get; set; }

    public string? CurrentPassword { get; set; }

    public string? newPassword { get; set; }
}