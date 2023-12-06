using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Classroom.Controllers.Hubs;
using Classroom.Data;
using Classroom.Models.Catalog.Rooms;

namespace Classroom.ApiControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;

        public RoomsController(ApplicationDbContext context,
            IMapper mapper,
            IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomViewModel>>> Get()
        {
            var rooms = await _context.Rooms.Include(x=> x.Messages).ToListAsync();

            var roomsViewModel = _mapper.Map<IEnumerable<Room>, IEnumerable<RoomViewModel>>(rooms);

            return Ok(roomsViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> Get(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound();

            var roomViewModel = _mapper.Map<Room, RoomViewModel>(room);
            return Ok(roomViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Room>> Create(RoomViewModel roomViewModel)
        {
            if (_context.Rooms.Any(r => r.Name == roomViewModel.Name))
                return BadRequest("Invalid room name or room already exists");

            var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var room = new Room()
            {
                Name = roomViewModel.Name,
                Admin = user
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("addChatRoom", new { id = room.Id, name = room.Name });

            return CreatedAtAction(nameof(Get), new { id = room.Id }, new { id = room.Id, name = room.Name });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roomViewModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, RoomViewModel roomViewModel)
        {
            if (_context.Rooms.Any(r => r.Name == roomViewModel.Name))
                return BadRequest("Invalid room name or room already exists");

            var room = await _context.Rooms
                .Include(r => r.Admin)
                .Where(r => r.Id == id && r.Admin.UserName == User.Identity.Name)
                .FirstOrDefaultAsync();

            if (room == null)
                return NotFound();

            room.Name = roomViewModel.Name;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("updateChatRoom", new { id = room.Id, room.Name});

            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Admin)
                .Where(r => r.Id == id && r.Admin.UserName == User.Identity.Name)
                .FirstOrDefaultAsync();

            if (room == null)
                return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("removeChatRoom", room.Id);
            await _hubContext.Clients.Group(room.Name).SendAsync("onRoomDeleted", string.Format("Phòng trò chuyện {0} đã được xoá!", room.Name));

            return Ok();
        }
    }
}
