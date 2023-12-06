namespace Classroom.Models.System.Users;

/// <summary>
/// ResetPasswordRequest
/// </summary>
public class ResetPasswordRequest
{
    public string? UserName { get; set; }

    public string? CurrentPassword { get; set; }

    public string? newPassword { get; set; }
}