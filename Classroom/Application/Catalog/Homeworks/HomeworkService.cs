using System.Net.Http.Headers;
using Classroom.Application.Common;
using Classroom.Data;
using Classroom.Models.Catalog.Homeworks;
using Classroom.Models.Common;
using Classroom.Utilities.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Classroom.Application.Catalog.Homeworks;

public class HomeworkService : IHomeworkService
{
    private readonly ApplicationDbContext _context;
    private readonly IStorageService _storageService;
    private readonly UserManager<ApplicationUser> _userManager;
    private const string USER_CONTENT_FOLDER_NAME = "user-content";
    public HomeworkService(ApplicationDbContext context,
                           IStorageService storageService,
                           UserManager<ApplicationUser> userManager)
    {
        _storageService = storageService;
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<int> Update(HomeworkUpdateRequest request)
    {
        var homework = await _context.Homeworks.FindAsync(request.HomeworkID);
        if (homework == null) throw new ClassroomException($"Cannot find a homework {request.HomeworkID}");
        homework.HomeworkName = request.HomeworkName;
        homework.Description = request.Description;
        homework.Deadline = request.Deadline;
        homework.SubmissionDateTime = request.SubmissionDateTime;

        if (request.ThumbnailImages != null)
        {
            var nImages = await _context.HomeworkImages.Where(x => x.HomeworkID == request.HomeworkID).ToListAsync();

            foreach (var item in nImages)
            {
                await _storageService.DeleteFileAsync(item.ImagePath.Replace("/user-content/", ""));
                _context.Remove(item);
                await _context.SaveChangesAsync();
            }
            foreach (var item in request.ThumbnailImages)
            {
                var image = new HomeworkImage()
                {
                    HomeworkID = homework.HomeworkID,
                    ImageFileSize = item.Length,
                    ImagePath = await this.SaveFile(item)
                };
                _context.HomeworkImages.Add(image);
                await _context.SaveChangesAsync();
            }
        }
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="HomeworkID"></param>
    /// <returns></returns>
    public async Task<HomeworkViewModel> GetById(int HomeworkID)
    {
        var homework = await _context.Homeworks.FindAsync(HomeworkID);
        if (homework == null) throw new ClassroomException($"Cannot find a homework {HomeworkID}");

        var _class = await _context.Classes.FindAsync(homework.ClassID);
        if (_class == null) throw new ClassroomException($"Cannot find a class {homework.ClassID}");

        var classDetail = await _context.ClassDetails.FirstOrDefaultAsync(x => x.ClassID == _class.ID && x.IsTeacher == Data.Enums.Teacher.Teacher);
        if (classDetail == null) throw new ClassroomException($"Cannot find a class {homework.ClassID}");

        var homeworkViewModel = new HomeworkViewModel()
        {
            HomeworkID = homework.HomeworkID,
            ID = _class.ID,
            ClassName = _class.ClassName,
            ClassID = _class.ID,
            HomeworkName = homework.HomeworkName,
            Description = homework.Description,
            DateTimeCreated = homework.DateTimeCreated,
            SubmissionDateTime = homework.SubmissionDateTime,
            Deadline = homework.Deadline,
            TeacherID = classDetail.UserID,
            HomeworkImages = await _context.HomeworkImages.Where(x => x.HomeworkID == homework.HomeworkID).ToListAsync(),
            Submissions = await _context.Submissions.Where(x => x.HomeworkID == homework.HomeworkID).ToListAsync()
        };

        foreach (var item in homeworkViewModel.Submissions)
        {
            var _user = await _userManager.FindByIdAsync(item.StudentID);
            item.Student = _user;
        }
        return homeworkViewModel;
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
    public async Task<int> Create(HomeworkCreateRequest request)
    {
        var homework = new Homework()
        {
            ClassID = request.ClassID,
            HomeworkName = request.HomeworkName,
            Description = request.Description,
            DateTimeCreated = DateTime.Now,
            Deadline = request.Deadline,
            SubmissionDateTime = request.SubmissionDateTime
        };
        _context.Homeworks.Add(homework);
        await _context.SaveChangesAsync();
        // Save file
        if (request.ThumbnailImages != null)

            foreach (var item in request.ThumbnailImages)
            {
                var image = new HomeworkImage()
                {
                    HomeworkID = homework.HomeworkID,
                    ImageFileSize = item.Length,
                    ImagePath = await this.SaveFile(item)
                };
                _context.HomeworkImages.Add(image);
                await _context.SaveChangesAsync();
            }
        return homework.HomeworkID;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="HomeworkID"></param>
    /// <returns></returns>
    public async Task<int> Delete(int HomeworkID)
    {
        var homework = await _context.Homeworks.FindAsync(HomeworkID);
        if (homework == null) throw new ClassroomException($"Cannot find a homework {HomeworkID}");

        _context.Homeworks.Remove(homework);
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<PagedResult<HomeworkViewModel>> GetAllPaging(GetManageHomeworkPagingRequest request)
    {
        //1. Select join
        var query = from hw in _context.Homeworks
                    join c in _context.Classes on hw.ClassID equals c.ID into hwc
                    from c in hwc.DefaultIfEmpty()
                    select new { hw, c };
        //2. filter
        if (!string.IsNullOrEmpty(request.Keyword))
            query = query.Where(x => x.hw.HomeworkName.Contains(request.Keyword)
                || x.c.ClassID.Contains(request.Keyword));

        if (request.ClassID != null && request.ClassID != 0)
        {
            query = query.Where(p => p.hw.ClassID == request.ClassID);
        }

        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new HomeworkViewModel()
            {
                HomeworkID = x.hw.HomeworkID,
                ID = x.c.ID,
                ClassName = x.c.ClassName,
                HomeworkName = x.hw.HomeworkName,
                Description = x.hw.Description,
                DateTimeCreated = x.hw.DateTimeCreated,
                Deadline = x.hw.Deadline,
                HomeworkImages = _context.HomeworkImages.Where(p => p.HomeworkID == x.hw.HomeworkID).ToList()
            }).ToListAsync();

        //4. Select and projection
        var pagedResult = new PagedResult<HomeworkViewModel>()
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
    public async Task<PagedResult<HomeworkViewModel>> GetAllMyHomeworkPaging(GetManageHomeworkPagingRequest request)
    {
        //1. Select join
        var query = from hw in _context.Homeworks.Include(x => x.Submissions)
                    join c in _context.Classes.Include(x => x.ClassDetails) on hw.ClassID equals c.ID into hwc
                    from c in hwc.DefaultIfEmpty()
                    select new { hw, c };
        //2. filter
        if (!string.IsNullOrEmpty(request.Keyword))
            query = query.Where(x => x.hw.HomeworkName.Contains(request.Keyword)
                || x.c.ClassID.Contains(request.Keyword));

        if (request.ClassID != null && request.ClassID != 0)
        {
            query = query.Where(p => p.hw.ClassID == request.ClassID);
        }

        if (request.UserId != null)
        {
            query = query.Where(x => x.c.ClassDetails.Any(x => x.UserID == request.UserId));
        }

        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new HomeworkViewModel()
            {
                HomeworkID = x.hw.HomeworkID,
                ID = x.c.ID,
                ClassName = x.c.ClassName,
                HomeworkName = x.hw.HomeworkName,
                Description = x.hw.Description,
                DateTimeCreated = x.hw.DateTimeCreated,
                SubmissionDateTime = x.hw.SubmissionDateTime,
                Deadline = x.hw.Deadline,
                HomeworkImages = _context.HomeworkImages.Where(p => p.HomeworkID == x.hw.HomeworkID).ToList(),
                MySubmission = _context.Submissions.FirstOrDefault(p => p.HomeworkID == x.hw.HomeworkID && p.StudentID == request.UserId)
            }).OrderByDescending(x => x.DateTimeCreated).ToListAsync();

        //4. Select and projection
        var pagedResult = new PagedResult<HomeworkViewModel>()
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
    public async Task<PagedResult<HomeworkViewModel>> GetAllMyAdminHomeworkPaging(GetManageHomeworkPagingRequest request)
    {
        //1. Select join
        var query = from hw in _context.Homeworks.Include(x => x.Submissions)
                    join c in _context.Classes.Include(x => x.ClassDetails) on hw.ClassID equals c.ID into hwc
                    from c in hwc.DefaultIfEmpty()
                    select new { hw, c };
        //2. filter
        if (!string.IsNullOrEmpty(request.Keyword))
            query = query.Where(x => x.hw.HomeworkName.Contains(request.Keyword)
                || x.c.ClassID.Contains(request.Keyword));

        if (request.ClassID != null && request.ClassID != 0)
        {
            query = query.Where(p => p.hw.ClassID == request.ClassID);
        }

        if (request.UserId != null)
        {
            query = query.Where(x => x.c.ClassDetails.Any(x => x.UserID == request.UserId && x.IsTeacher == Data.Enums.Teacher.Teacher));
        }

        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new HomeworkViewModel()
            {
                HomeworkID = x.hw.HomeworkID,
                ID = x.c.ID,
                ClassName = x.c.ClassName,
                HomeworkName = x.hw.HomeworkName,
                Description = x.hw.Description,
                DateTimeCreated = x.hw.DateTimeCreated,
                SubmissionDateTime = x.hw.SubmissionDateTime,
                Deadline = x.hw.Deadline,
                HomeworkImages = _context.HomeworkImages.Where(p => p.HomeworkID == x.hw.HomeworkID).ToList(),
                MySubmission = _context.Submissions.FirstOrDefault(p => p.HomeworkID == x.hw.HomeworkID && p.StudentID == request.UserId)
            }).OrderByDescending(x => x.DateTimeCreated).ToListAsync();

        //4. Select and projection
        var pagedResult = new PagedResult<HomeworkViewModel>()
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
    /// <param name="UserId"></param>
    /// <returns></returns>
    public async Task<List<HomeworkViewModel>> GetAllMyAdminHomework(string? UserId)
    {
        //1. Select join
        var query = from hw in _context.Homeworks.Include(x => x.Submissions)
                    join c in _context.Classes.Include(x => x.ClassDetails) on hw.ClassID equals c.ID into hwc
                    from c in hwc.DefaultIfEmpty()
                    select new { hw, c };

        if (UserId != null)
        {
            query = query.Where(x => x.c.ClassDetails.Any(x => x.UserID == UserId && x.IsTeacher == Data.Enums.Teacher.Teacher));
        }

        var data = await query
            .Select(x => new HomeworkViewModel()
            {
                HomeworkID = x.hw.HomeworkID,
                ClassName = x.c.ClassName,
                HomeworkName = x.hw.HomeworkName,
            }).OrderByDescending(x => x.HomeworkName).ToListAsync();

        return data;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<List<HomeworkViewModel>> GetAll()
    {
        //1. Select join
        var query = from hw in _context.Homeworks
                    join c in _context.Classes on hw.ClassID equals c.ID into hwc
                    from c in hwc.DefaultIfEmpty()
                    select new { hw, c };

        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query
            .Select(x => new HomeworkViewModel()
            {
                HomeworkID = x.hw.HomeworkID,
                ID = x.c.ID,
                ClassName = x.c.ClassName,
                HomeworkName = x.hw.HomeworkName,
                Description = x.hw.Description,
                DateTimeCreated = x.hw.DateTimeCreated,
                Deadline = x.hw.Deadline,
                HomeworkImages = _context.HomeworkImages.Where(p => p.HomeworkID == x.hw.HomeworkID).ToList()
            }).ToListAsync();
        return data;
    }
}

