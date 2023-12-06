using Microsoft.AspNetCore.Identity;

namespace Classroom.Core.Repositories
{
    /// <summary>
    /// IRoleRepository
    /// </summary>
    public interface IRoleRepository
    {
        ICollection<IdentityRole> GetRoles();
    }
}
