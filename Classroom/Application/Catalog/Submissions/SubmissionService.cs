using System.Net.Http.Headers;
using Classroom.Application.Common;
using Classroom.Data;
using Classroom.Data.Enums;
using Classroom.Models.Catalog.Submissions;
using Classroom.Models.Common;
using Classroom.Utilities.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Classroom.Application.Catalog.Submissions;

/// <summary>
/// SubmissionService
/// </summary>
public class SubmissionService : ISubmissionService
{
    private readonly IStorageService _storageService;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private const string USER_CONTENT_FOLDER_NAME = "user-content";

    public SubmissionService(ApplicationDbContext context,
                             UserManager<ApplicationUser> userManager,
                             IStorageService storageService)
    {
        _context = context;
        _userManager = userManager;
        _storageService = storageService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<bool> Create(SubmissionCreateRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.StudentID);
        var homework = await _context.Homeworks.FindAsync(request.HomeworkID);
        if (homework == null) throw new ClassroomException($"Cannot find a homework {request.HomeworkID}");

        if (_context.Submissions.FirstOrDefault(x => x.HomeworkID == request.HomeworkID && x.StudentID == user.Id) != null) return false;

        var submission = new Submission()
        {
            HomeworkID = request.HomeworkID,
            StudentID = user.Id,
            Description = request.Description,
            SubmissionDateTime = DateTime.Now,
            DateTimeUpdated = DateTime.Now
        };
        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();
        // Save file
        if (request.ThumbnailImages != null)

            foreach (var item in request.ThumbnailImages)
            {
                var image = new SubmissionImage()
                {
                    SubmissionID = submission.SubmissionID,
                    ImageFileSize = item.Length,
                    ImagePath = await this.SaveFile(item)
                };
                _context.SubmissionImages.Add(image);
                await _context.SaveChangesAsync();
            }
        return true;
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
    public async Task<bool> Update(SubmissionUpdateRequest request)
    {
        var submission = _context.Submissions.FirstOrDefault(x => x.SubmissionID == request.SubmissionID);
        if (submission == null) throw new ClassroomException($"Cannot find a submission {request.SubmissionID}");
        submission.Description = request.Description;
        submission.DateTimeUpdated = DateTime.Now;

        if (request.ThumbnailImages != null)
        {
            var nImages = await _context.SubmissionImages.Where(x => x.SubmissionID == request.SubmissionID).ToListAsync();

            foreach (var item in nImages)
            {
                await _storageService.DeleteFileAsync(item.ImagePath.Replace("/user-content/", ""));
                _context.Remove(item);
                await _context.SaveChangesAsync();
            }
            foreach (var item in request.ThumbnailImages)
            {
                var image = new SubmissionImage()
                {
                    SubmissionID = submission.SubmissionID,
                    ImageFileSize = item.Length,
                    ImagePath = await this.SaveFile(item)
                };
                _context.SubmissionImages.Add(image);
                await _context.SaveChangesAsync();
            }
        }

        await _context.SaveChangesAsync();
        return true;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="SubmissionID"></param>
    /// <returns></returns>
    public async Task<bool> Delete(int SubmissionID)
    {
        var submission = await _context.Submissions.FindAsync(SubmissionID);
        if (submission == null) throw new ClassroomException($"Cannot find a submission {SubmissionID}");

        _context.Submissions.Remove(submission);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="SubmissionID"></param>
    /// <returns></returns>
    public async Task<SubmissionViewModel> GetById(int SubmissionID)
    {
        var submission = await _context.Submissions.FindAsync(SubmissionID);
        if (submission == null) return null;

        var student = await _userManager.FindByIdAsync(submission.StudentID);

        var homework = await _context.Homeworks.FindAsync(submission.HomeworkID);
        if (homework == null) return null;

        var _class = await _context.Classes.FindAsync(homework.ClassID);
        if (_class == null) return null;

        var classDetail = await _context.ClassDetails.FirstOrDefaultAsync(x=> x.ClassID == _class.ID && x.IsTeacher == Teacher.Teacher);

        var homeworkViewModel = new SubmissionViewModel()
        {
            TeacherId = classDetail.UserID,
            SubmissionID = submission.SubmissionID,
            HomeworkID = homework.HomeworkID,
            StudentID = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            PhoneNumber = student.PhoneNumber,
            Email = student.Email,
            HomeworkName = homework.HomeworkName,
            ClassID = _class.ClassID,
            ID = _class.ID,
            ClassName = _class.ClassName,
            DescriptionHomework = homework.Description,
            DateTimeCreated = homework.DateTimeCreated,
            Deadline = homework.Deadline,
            Mark = submission.Mark,
            Note = submission.Note,
            Description = submission.Description,
            SubmissionDateTime = submission.SubmissionDateTime,
            DateTimeUpdated = submission.DateTimeUpdated,
            SubmissionImages = await _context.SubmissionImages.Where(x=> x.SubmissionID == submission.SubmissionID).ToListAsync()
        };
        return homeworkViewModel;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<PagedResult<SubmissionViewModel>> GetAllPaging(GetManageSubmissionPagingRequest request)
    {
        //1. Select join
        var query = from sm in _context.Submissions
                    join st in _userManager.Users on sm.StudentID equals st.Id into smst
                    from st in smst.DefaultIfEmpty()
                    join hw in _context.Homeworks on sm.HomeworkID equals hw.HomeworkID into smhw
                    from hw in smhw.DefaultIfEmpty()
                    join c in _context.Classes on hw.ClassID equals c.ID
                    select new { sm, st, hw, c };

        if(request.HomeworkID != 0 && request.HomeworkID != null){
            query = query.Where(x=> x.sm.HomeworkID == request.HomeworkID);
        }

        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query.OrderByDescending(x=> x.sm.DateTimeUpdated).Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SubmissionViewModel()
            {
                SubmissionID = x.sm.SubmissionID,
                HomeworkID = x.hw.HomeworkID,
                StudentID = x.st.Id,
                FirstName = x.st.FirstName,
                LastName = x.st.LastName,
                PhoneNumber = x.st.PhoneNumber,
                Email = x.st.Email,
                HomeworkName = x.hw.HomeworkName,
                ClassID = x.c.ClassID,
                ClassName = x.c.ClassName,
                DescriptionHomework = x.hw.Description,
                DateTimeCreated = x.hw.DateTimeCreated,
                Deadline = x.hw.Deadline,
                Mark = x.sm.Mark,
                Note = x.sm.Note,
                Description = x.sm.Description,
                SubmissionDateTime = x.sm.SubmissionDateTime,
                DateTimeUpdated = x.sm.DateTimeUpdated
            }).ToListAsync();

        //4. Select and projection
        var pagedResult = new PagedResult<SubmissionViewModel>()
        {
            TotalRecords = totalRow,
            PageSize = request.PageSize,
            PageIndex = request.PageIndex,
            Items = data
        };
        return pagedResult;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<PagedResult<SubmissionViewModel>> GetMyAllPaging(GetManageSubmissionPagingRequest request)
    {
        //1. Select join
        var query = from sm in _context.Submissions
                    join st in _userManager.Users on sm.StudentID equals st.Id into smst
                    from st in smst.DefaultIfEmpty()
                    join hw in _context.Homeworks on sm.HomeworkID equals hw.HomeworkID into smhw
                    from hw in smhw.DefaultIfEmpty()
                    join c in _context.Classes.Include(x=> x.ClassDetails) on hw.ClassID equals c.ID
                    select new { sm, st, hw, c };

        if(request.HomeworkID != 0 && request.HomeworkID != null){
            query = query.Where(x=> x.sm.HomeworkID == request.HomeworkID);
        }

        if(request.UserId != null){
            query = query.Where(x=> x.c.ClassDetails.Any(x=> x.UserID == request.UserId && x.IsTeacher == Data.Enums.Teacher.Teacher));
        }

        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query.OrderByDescending(x=> x.sm.DateTimeUpdated).Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SubmissionViewModel()
            {
                SubmissionID = x.sm.SubmissionID,
                HomeworkID = x.hw.HomeworkID,
                StudentID = x.st.Id,
                FirstName = x.st.FirstName,
                LastName = x.st.LastName,
                PhoneNumber = x.st.PhoneNumber,
                Email = x.st.Email,
                HomeworkName = x.hw.HomeworkName,
                ClassID = x.c.ClassID,
                ClassName = x.c.ClassName,
                DescriptionHomework = x.hw.Description,
                DateTimeCreated = x.hw.DateTimeCreated,
                Deadline = x.hw.Deadline,
                Mark = x.sm.Mark,
                Note = x.sm.Note,
                Description = x.sm.Description,
                SubmissionDateTime = x.sm.SubmissionDateTime,
                DateTimeUpdated = x.sm.DateTimeUpdated
            }).ToListAsync();

        //4. Select and projection
        var pagedResult = new PagedResult<SubmissionViewModel>()
        {
            TotalRecords = totalRow,
            PageSize = request.PageSize,
            PageIndex = request.PageIndex,
            Items = data
        };
        return pagedResult;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<PagedResult<SubmissionViewModel>> GetMyAll(GetManageSubmissionPagingRequest request)
    {
        //1. Select join
        var query = from sm in _context.Submissions
                    join st in _userManager.Users on sm.StudentID equals st.Id into smst
                    from st in smst.DefaultIfEmpty()
                    join hw in _context.Homeworks on sm.HomeworkID equals hw.HomeworkID into smhw
                    from hw in smhw.DefaultIfEmpty()
                    join c in _context.Classes.Include(x=> x.ClassDetails) on hw.ClassID equals c.ID
                    select new { sm, st, hw, c };

        if(request.HomeworkID != 0 && request.HomeworkID != null){
            query = query.Where(x=> x.sm.HomeworkID == request.HomeworkID);
        }

        if(request.UserId != null){
            query = query.Where(x=> x.c.ClassDetails.Any(x=> x.UserID == request.UserId && x.IsTeacher == Data.Enums.Teacher.Teacher));
        }

        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query.OrderByDescending(x=> x.sm.DateTimeUpdated)
            .Select(x => new SubmissionViewModel()
            {
                SubmissionID = x.sm.SubmissionID,
                HomeworkID = x.hw.HomeworkID,
                StudentID = x.st.Id,
                FirstName = x.st.FirstName,
                LastName = x.st.LastName,
                PhoneNumber = x.st.PhoneNumber,
                Email = x.st.Email,
                HomeworkName = x.hw.HomeworkName,
                ClassID = x.c.ClassID,
                ClassName = x.c.ClassName,
                DescriptionHomework = x.hw.Description,
                DateTimeCreated = x.hw.DateTimeCreated,
                Deadline = x.hw.Deadline,
                Mark = x.sm.Mark,
                Note = x.sm.Note,
                Description = x.sm.Description,
                SubmissionDateTime = x.sm.SubmissionDateTime,
                DateTimeUpdated = x.sm.DateTimeUpdated
            }).ToListAsync();

        //4. Select and projection
        var pagedResult = new PagedResult<SubmissionViewModel>()
        {
            TotalRecords = totalRow,
            PageSize = request.PageSize,
            PageIndex = request.PageIndex,
            Items = data
        };
        return pagedResult;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<bool> UpdateMark(SubmissionUpdateMarkRequest request)
    {
        var submission = _context.Submissions.FirstOrDefault(x => x.SubmissionID == request.SubmissionID);
        if (submission == null) throw new ClassroomException($"Cannot find a submission {request.SubmissionID}");
        submission.Mark = request.Mark;
        submission.Note = request.Note;
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return true;
        }
        return false;
    }
}

