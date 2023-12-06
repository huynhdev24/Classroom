namespace Classroom.Models.System.Users;

/// <summary>
/// LoginRequest
/// </summary>
public class LoginRequest
{
    public string? UserName { get; set; }

    public string? Password { get; set; }

    public bool RememberMe { get; set; }
}