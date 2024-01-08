using Microsoft.AspNetCore.Identity;

namespace Classroom.Core.Repositories
{
    /// <summary>
    /// IRoleRepository
    /// </summary>
    /// <author>huynhdev24</author>
    public interface IRoleRepository
    {
        ICollection<IdentityRole> GetRoles();
    }
}
