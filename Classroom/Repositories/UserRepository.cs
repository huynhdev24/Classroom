using Classroom.Core.Repositories;
using Classroom.Data;

namespace Classroom.Repositories
{
    /// <summary>
    /// UserRepository
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ICollection<ApplicationUser> GetUsers(string? keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                var users = _context.Users.Where(x=> x.FirstName.Contains(keyword)
                || x.LastName.Contains(keyword)
                || x.Email.Contains(keyword)
                || x.PhoneNumber.Contains(keyword)).OrderBy(x => x.LastName).ToList();
                return users;
            }
            return _context.Users.OrderBy(x => x.LastName).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApplicationUser?> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ApplicationUser UpdateUser(ApplicationUser user)
        {
            _context.Update(user);
            _context.SaveChanges();

            return user;
        }
    }
}
