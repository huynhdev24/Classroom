using Classroom.Data;
using Classroom.Core.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Classroom.Repositories
{
    /// <summary>
    /// RoleRepository
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICollection<IdentityRole> GetRoles()
        {
            return _context.Roles.ToList();
        }
    }
}
