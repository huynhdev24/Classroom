using Classroom.Data;
using Classroom.Data.Enums;
using Classroom.Models.Catalog.ExamSchedules;
using Classroom.Models.Common;
using Classroom.Utilities.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Classroom.Application.Catalog.ExamSchedules;

public class ExamScheduleService : IExamScheduleService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public ExamScheduleService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<int> Create(ExamSchedulesCreateRequest request)
    {
        var examSchedule = new ExamSchedule()
        {
            ClassID = request.ClassID,
            ExamScheduleName = request.ExamScheduleName,
            ExamDateTime = request.ExamDateTime,
            ExamTime = request.ExamTime,
            DateTimeCreated = DateTime.Now,
            Description = request.Description,
            Deadline = request.Deadline
        };
        _context.ExamSchedules.Add(examSchedule);
        await _context.SaveChangesAsync();
        return examSchedule.ExamScheduleID;
    }

    public async Task<int> Delete(int ExamScheduleID)
    {
        var examSchedule = await _context.ExamSchedules.FindAsync(ExamScheduleID);
        if (examSchedule == null) throw new ClassroomException($"Cannot find a class {ExamScheduleID}");

        _context.ExamSchedules.Remove(examSchedule);
        return await _context.SaveChangesAsync();
    }

    public async Task<List<ExamSchedulesViewModel>> GetAllMyExamSchedules(string UserName)
    {
        var user = await _userManager.FindByNameAsync(UserName);
        //1. Select join
        var query = from es in _context.ExamSchedules
                    join c in _context.Classes on es.ClassID equals c.ID into esc
                    from c in esc.DefaultIfEmpty()
                    join cd in _context.ClassDetails on c.ID equals cd.ClassID into ccd
                    from cd in ccd.DefaultIfEmpty()
                    where cd.IsTeacher == Data.Enums.Teacher.Teacher && cd.UserID == user.Id
                    select new { es, c };
        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query
            .Select(x => new ExamSchedulesViewModel()
            {
                ExamScheduleID = x.es.ExamScheduleID,
                ClassID = x.c.ID,
                ExamScheduleName = x.es.ExamScheduleName + " - " + x.c.ClassName,
                DateTimeCreated = x.es.DateTimeCreated,
                ExamDateTime = x.es.ExamDateTime,
                ExamTime = x.es.ExamTime,
                Description = x.es.Description,
                Deadline = x.es.Deadline
            }).OrderByDescending(x => x.ExamDateTime).ToListAsync();
        return data;
    }

    public async Task<PagedResult<ExamSchedulesViewModel>> GetAllPaging(GetManageExamSchedulesPagingRequest request)
    {
        //1. Select join
        var query = from es in _context.ExamSchedules
                    join c in _context.Classes on es.ClassID equals c.ID into esc
                    from c in esc.DefaultIfEmpty()
                    select new { es, c };
        //2. filter
        if (!string.IsNullOrEmpty(request.Keyword))
            query = query.Where(x => x.es.ExamScheduleName.Contains(request.Keyword)
                || x.c.ClassID.Contains(request.Keyword));

        if (request.ClassID != null && request.ClassID != 0)
        {
            query = query.Where(p => p.es.ClassID == request.ClassID);
        }

        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ExamSchedulesViewModel()
            {
                ExamScheduleID = x.es.ExamScheduleID,
                ClassID = x.c.ID,
                ClassName = x.c.ClassName,
                ExamScheduleName = x.es.ExamScheduleName,
                DateTimeCreated = x.es.DateTimeCreated,
                ExamDateTime = x.es.ExamDateTime,
                ExamTime = x.es.ExamTime,
                Description = x.es.Description,
                Deadline = x.es.Deadline
            }).OrderByDescending(x => x.ExamDateTime).ToListAsync();

        //4. Select and projection
        var pagedResult = new PagedResult<ExamSchedulesViewModel>()
        {
            TotalRecords = totalRow,
            PageSize = request.PageSize,
            PageIndex = request.PageIndex,
            Items = data
        };
        return pagedResult;
    }

    public async Task<PagedResult<ExamSchedulesViewModel>> GetAllMyExamSchedulesPaging(GetManageExamSchedulesPagingRequest request)
    {
        //1. Select join
        var query = from es in _context.ExamSchedules
                    join c in _context.Classes.Include(x => x.ClassDetails) on es.ClassID equals c.ID into esc
                    from c in esc.DefaultIfEmpty()
                    select new { es, c };
        //2. filter
        if (!string.IsNullOrEmpty(request.Keyword))
            query = query.Where(x => x.es.ExamScheduleName.Contains(request.Keyword)
                || x.c.ClassID.Contains(request.Keyword));

        if (request.ClassID != null && request.ClassID != 0)
        {
            query = query.Where(p => p.es.ClassID == request.ClassID);
        }

        if (request.UserId != null)
        {
            query = query.Where(x => x.c.ClassDetails.Any(x => x.UserID == request.UserId));
        }

        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ExamSchedulesViewModel()
            {
                ExamScheduleID = x.es.ExamScheduleID,
                ClassID = x.c.ID,
                ClassName = x.c.ClassName,
                ExamScheduleName = x.es.ExamScheduleName,
                DateTimeCreated = x.es.DateTimeCreated,
                ExamDateTime = x.es.ExamDateTime,
                ExamTime = x.es.ExamTime,
                Description = x.es.Description,
                Deadline = x.es.Deadline,
                MyStudentExam = _context.StudentExams.FirstOrDefault(p => p.ExamScheduleID == x.es.ExamScheduleID && p.StudentID == request.UserId)
            }).OrderByDescending(x => x.ExamDateTime).ToListAsync();

        //4. Select and projection
        var pagedResult = new PagedResult<ExamSchedulesViewModel>()
        {
            TotalRecords = totalRow,
            PageSize = request.PageSize,
            PageIndex = request.PageIndex,
            Items = data
        };
        return pagedResult;
    }

    public async Task<PagedResult<ExamSchedulesViewModel>> GetAllMyExamAdminSchedulesPaging(GetManageExamSchedulesPagingRequest request)
    {
        //1. Select join
        var query = from es in _context.ExamSchedules
                    join c in _context.Classes.Include(x => x.ClassDetails) on es.ClassID equals c.ID into esc
                    from c in esc.DefaultIfEmpty()
                    select new { es, c };
        //2. filter
        if (!string.IsNullOrEmpty(request.Keyword))
            query = query.Where(x => x.es.ExamScheduleName.Contains(request.Keyword)
                || x.c.ClassID.Contains(request.Keyword));

        if (request.ClassID != null && request.ClassID != 0)
        {
            query = query.Where(p => p.es.ClassID == request.ClassID);
        }

        if (request.UserId != null)
        {
            query = query.Where(x => x.c.ClassDetails.Any(x => x.UserID == request.UserId && x.IsTeacher == Data.Enums.Teacher.Teacher));
        }

        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ExamSchedulesViewModel()
            {
                ExamScheduleID = x.es.ExamScheduleID,
                ClassName = x.c.ClassName,
                ExamScheduleName = x.es.ExamScheduleName,
                DateTimeCreated = x.es.DateTimeCreated,
                ExamDateTime = x.es.ExamDateTime,
                ExamTime = x.es.ExamTime,
                Deadline = x.es.Deadline,
            }).OrderByDescending(x => x.ExamDateTime).ToListAsync();

        //4. Select and projection
        var pagedResult = new PagedResult<ExamSchedulesViewModel>()
        {
            TotalRecords = totalRow,
            PageSize = request.PageSize,
            PageIndex = request.PageIndex,
            Items = data
        };
        return pagedResult;
    }

    public async Task<List<ExamSchedulesViewModel>> GetAllMyExamAdminSchedules(string UserId)
    {
        //1. Select join
        var query = from es in _context.ExamSchedules
                    join c in _context.Classes.Include(x => x.ClassDetails) on es.ClassID equals c.ID into esc
                    from c in esc.DefaultIfEmpty()
                    select new { es, c };


        if (UserId != null)
        {
            query = query.Where(x => x.c.ClassDetails.Any(x => x.UserID == UserId && x.IsTeacher == Data.Enums.Teacher.Teacher));
        }

        var data = await query
            .Select(x => new ExamSchedulesViewModel()
            {
                ExamScheduleID = x.es.ExamScheduleID,
                ClassName = x.c.ClassName,
                ExamScheduleName = x.es.ExamScheduleName,
                DateTimeCreated = x.es.DateTimeCreated,
                ExamDateTime = x.es.ExamDateTime,
                ExamTime = x.es.ExamTime,
                Deadline = x.es.Deadline,
            }).OrderByDescending(x => x.ExamDateTime).ToListAsync();

        return data;
    }

    public async Task<ExamSchedulesViewModel> GetById(int ExamScheduleID)
    {
        var examSchedule = await _context.ExamSchedules.FindAsync(ExamScheduleID);
        if (examSchedule == null) return null;

        var _class = await _context.Classes.FindAsync(examSchedule.ClassID);
        if (_class == null) return null;
        var classDetail = _context.ClassDetails.FirstOrDefault(x => x.ClassID == _class.ID && x.IsTeacher == Teacher.Teacher);
        classDetail.User = await _userManager.FindByIdAsync(classDetail.UserID);
        var examScheduleViewModel = new ExamSchedulesViewModel()
        {
            TeacherID = classDetail.User.Id,
            ExamScheduleID = examSchedule.ExamScheduleID,
            ClassID = _class.ID,
            ClassName = _class.ClassName,
            ExamScheduleName = examSchedule.ExamScheduleName,
            DateTimeCreated = examSchedule.DateTimeCreated,
            ExamDateTime = examSchedule.ExamDateTime,
            ExamTime = examSchedule.ExamTime,
            Description = examSchedule.Description,
            Deadline = examSchedule.Deadline
        };
        return examScheduleViewModel;
    }

    public async Task<int> Update(ExamSchedulesUpdateRequest request)
    {
        var examSchedule = await _context.ExamSchedules.FindAsync(request.ExamScheduleID);
        if (examSchedule == null) throw new ClassroomException($"Cannot find a exam schedule {request.ExamScheduleID}");
        examSchedule.ExamScheduleName = request.ExamScheduleName;
        examSchedule.ExamDateTime = request.ExamDateTime;
        examSchedule.ExamTime = request.ExamTime;
        examSchedule.Description = request.Description;
        examSchedule.Deadline = request.Deadline;
        return await _context.SaveChangesAsync();
    }
}