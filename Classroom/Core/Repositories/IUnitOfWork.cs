namespace Classroom.Core.Repositories
{
    /// <summary>
    /// IUnitOfWork
    /// </summary>
    /// <author>huynhdev24</author>
    public interface IUnitOfWork
    {
        IUserRepository User { get; }

        IRoleRepository Role { get; }
    }
}
