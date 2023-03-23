using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using AutoMapper;
using Classroom.Application.Common;
using Classroom.Controllers.Hubs;
using Classroom.Data;
using Classroom.Models.Catalog.Messages;
using Classroom.Models.Catalog.Uploads;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Classroom.ApiControllers;

//[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UploadsController : ControllerBase
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IStorageService _storageService;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private const string USER_CONTENT_FOLDER_NAME = "user-content";

    public UploadsController(IHubContext<ChatHub> hubContext,
                             IStorageService storageService,
                             ApplicationDbContext context,
                             UserManager<ApplicationUser> userManager,
                             IMapper mapper)
    {
        _hubContext = hubContext;
        _storageService = storageService;
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }

    private async Task<string> SaveFile(IFormFile file)
    {
        var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
        await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
        return "/" + USER_CONTENT_FOLDER_NAME + "/" + fileName;
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<ActionResult<Message>> Upload([FromForm] UploadViewModel uploadViewModel)
    {
        if (ModelState.IsValid)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var room = _context.Rooms.FirstOrDefault(x => x.Id == uploadViewModel.RoomId);

            string htmlImage = string.Format(
                    "<a href=\"{0}\" target=\"_blank\">" +
                    "<img src=\"{0}\" class=\"post-image\">" +
                    "</a>", await this.SaveFile(uploadViewModel.File));

            var msg = new Message()
            {
                Content = Regex.Replace(htmlImage, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                FromUser = user,
                ToRoom = room,
                Timestamp = DateTime.Now
            };

            await _context.Messages.AddAsync(msg);
            await _context.SaveChangesAsync();

            MessageViewModel createdMessage = new MessageViewModel();
            createdMessage.Id = msg.Id;
            createdMessage.Content = msg.Content;
            createdMessage.Timestamp = msg.Timestamp;
            createdMessage.Room = room.Name;
            createdMessage.From = user.FirstName + " " + user.LastName;
            
            // Broadcast the message
            await _hubContext.Clients.Group(room.Name).SendAsync("newMessage", createdMessage);

            return Ok();
        }

        return BadRequest();
    }
}
