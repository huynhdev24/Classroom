using Classroom.Data;
using Classroom.Models.Catalog.Classes;

namespace Classroom.Application.Catalog.Rooms
{
    /// <summary>
    /// IRoomService
    /// </summary>
    public interface IRoomService
    {
        Task<int> Create(string UserName, string ClassName);
    }
}