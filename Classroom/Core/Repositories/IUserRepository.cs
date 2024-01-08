using Classroom.Data;

namespace Classroom.Core.Repositories;

/// <summary>
/// IUserRepository
/// </summary>
/// <author>huynhdev24</author>
public interface IUserRepository
{
    ICollection<ApplicationUser> GetUsers(string? keyword);

    Task<ApplicationUser?> GetUser(string id);

    ApplicationUser UpdateUser(ApplicationUser user);
}
