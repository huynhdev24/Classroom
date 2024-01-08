using Classroom.Application.Common;
using Microsoft.AspNetCore.Identity;
using Classroom.Data;
using AutoMapper;

namespace Classroom.Application.Catalog.Rooms
{
    /// <summary>
    /// RoomService
    /// </summary>
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="storageService"></param>
        /// <param name="mapper"></param>
        /// <author>huynhdev24</author>
        public RoomService(ApplicationDbContext context
        , IStorageService storageService
        , IMapper mapper)
        {
            _context = context;
            _storageService = storageService;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="ClassName"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        public async Task<int> Create(string UserName, string ClassName)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == UserName);
            var room = new Room()
            {
                Name = ClassName,
                Admin = user
            };  
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room.Id;
        }
    }
}

