using Microsoft.AspNetCore.Identity;

namespace Classroom.Core.Repositories
{
    public interface IRoleRepository
    {
        ICollection<IdentityRole> GetRoles();
    }
}
