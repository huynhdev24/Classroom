using Classroom.Application.Common;
using Microsoft.AspNetCore.Identity;
using Classroom.Data;
using AutoMapper;

namespace Classroom.Application.Catalog.Rooms
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";

        public RoomService(ApplicationDbContext context
        , IStorageService storageService
        , IMapper mapper)
        {
            _context = context;
            _storageService = storageService;
            _mapper = mapper;
        }

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

