namespace Classroom.Application.Common.SignalR;

/// <summary>
/// IFileValidator
/// </summary>
public interface IFileValidator
{
    bool IsValid(IFormFile file);
}
