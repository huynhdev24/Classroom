using Classroom.Utilities.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Classroom.Application.Common;
using Microsoft.AspNetCore.Identity;
using Classroom.Data;
using Classroom.Models.Catalog.Comments;
using Classroom.Models.Common;

namespace Classroom.Application.Catalog.Comments;

/// <summary>
/// CommentService
/// </summary>
public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStorageService _storageService;
    private const string USER_CONTENT_FOLDER_NAME = "user-content";
    public CommentService(ApplicationDbContext context, IStorageService storageService, UserManager<ApplicationUser> userManager)
    {
        _storageService = storageService;
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="CommentID"></param>
    /// <returns></returns>
    public async Task<CommentViewModel> GetById(int CommentID)
    {
        var comment = await _context.Comments.FindAsync(CommentID);
        if (comment == null) throw new ClassroomException($"Cannot find a comment {CommentID}");

        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == comment.UserID);

        var commentViewModel = new CommentViewModel()
        {
            CommentID = comment.CommentID,
            NotificationID = comment.NotificationID,
            UserID = comment.UserID,
            FullName = user.FirstName + " " + user.LastName,
            Content = comment.Content,
            DateTimeCreated = comment.DateTimeCreated,
            UserName = user.UserName
        };
        return commentViewModel;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<int> Update(CommentUpdateRequest request)
    {
        var comment = await _context.Comments.FindAsync(request.CommentID);
        if (comment == null) throw new ClassroomException($"Cannot find a comment {request.CommentID}");
        comment.Content = request.Content;

        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    private async Task<string> SaveFile(IFormFile file)
    {
        var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
        await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
        return "/" + USER_CONTENT_FOLDER_NAME + "/" + fileName;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<int> Create(CommentCreateRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        var comment = new Comment()
        {
            NotificationID = request.NotificationID,
            UserID = user.Id,
            Content = request.Content,
            DateTimeCreated = DateTime.Now
        };
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        
        return comment.CommentID;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="CommentID"></param>
    /// <returns></returns>
    public async Task<int> Delete(int CommentID)
    {
        var comment = await _context.Comments.FindAsync(CommentID);
        if (comment == null) throw new ClassroomException($"Cannot find a comment {CommentID}");

        _context.Comments.Remove(comment);
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<PagedResult<CommentViewModel>> GetAllPaging(GetManageCommentPagingRequest request)
    {
        //1. Select join
        var query = from c in _context.Comments
                    join u in _userManager.Users on c.UserID equals u.Id into cu
                    from u in cu.DefaultIfEmpty()
                    select new { u, c };
        //2. filter
        if (request.NotificationID != null && request.NotificationID != 0)
        {
            query = query.Where(p => p.c.NotificationID == request.NotificationID);
        }

        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query
            .Select(x => new CommentViewModel()
            {
                CommentID = x.c.CommentID,
                NotificationID = x.c.NotificationID,
                UserID = x.c.UserID,
                FullName = x.u.FirstName + " " + x.u.LastName,
                Content = x.c.Content,
                DateTimeCreated = x.c.DateTimeCreated,
                UserName = x.u.UserName
            }).ToListAsync();

        //4. Select and projection
        var pagedResult = new PagedResult<CommentViewModel>()
        {
            TotalRecords = totalRow,
            PageSize = request.PageSize,
            PageIndex = request.PageIndex,
            Items = data
        };
        return pagedResult;
    }
}

