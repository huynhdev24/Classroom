using Classroom.Utilities.Exceptions;
using Classroom.Models.Common;
using Microsoft.EntityFrameworkCore;
using Classroom.Models.Catalog.StudentExams;
using Microsoft.AspNetCore.Identity;
using Classroom.Data;

namespace Classroom.Application.Catalog.StudentExams;

public class StudentExamService : IStudentExamService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentExamService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<int> Create(StudentExamsCreateRequest request)
    {
        var student = await _userManager.FindByNameAsync(request.UserName);
        var studentExam = new StudentExam()
        {
            ExamScheduleID = request.ExamScheduleID,
            StudentID = student.Id,
            Mark = request.Mark,
            Note = request.Note,
            StudentExamDateTime = request.DateTimeStudentExam,
            SubmissionDateTime = DateTime.Now
        };
        _context.StudentExams.Add(studentExam);
        await _context.SaveChangesAsync();
        return studentExam.StudentExamID;
    }

    public async Task<int> Delete(int StudentExamID)
    {
        var studentExam = await _context.StudentExams.FindAsync(StudentExamID);
        if (studentExam == null) throw new ClassroomException($"Cannot find a student exam {StudentExamID}");

        _context.StudentExams.Remove(studentExam);
        return await _context.SaveChangesAsync();
    }

    public async Task<PagedResult<StudentExamsViewModel>> GetAllPaging(GetManageStudentExamPagingRequest request)
    {
        //1. Select join
        var query = from se in _context.StudentExams
                    join u in _userManager.Users on se.StudentID equals u.Id into seu
                    from u in seu.DefaultIfEmpty()
                    select new { u, se };
        //2. filter
        if (request.ExamScheduleID != null && request.ExamScheduleID != 0)
        {
            query = query.Where(p => p.se.ExamScheduleID == request.ExamScheduleID);
        }

        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new StudentExamsViewModel()
            {
                StudentExamID = x.se.StudentExamID,
                ExamScheduleID = x.se.ExamScheduleID,
                Mark = x.se.Mark,
                Note = x.se.Note,
                DateTimeStudentExam = x.se.StudentExamDateTime
            }).ToListAsync();

        //4. Select and projection
        var pagedResult = new PagedResult<StudentExamsViewModel>()
        {
            TotalRecords = totalRow,
            PageSize = request.PageSize,
            PageIndex = request.PageIndex,
            Items = data
        };
        return pagedResult;
    }

    public async Task<PagedResult<StudentExamsViewModel>> GetAllAdminPaging(GetManageStudentExamPagingRequest request)
    {
        //1. Select join
        var query = from se in _context.StudentExams
                    join es in _context.ExamSchedules on se.ExamScheduleID equals es.ExamScheduleID
                    join c in _context.Classes.Include(x=> x.ClassDetails) on es.ClassID equals c.ID
                    join u in _userManager.Users on se.StudentID equals u.Id into seu
                    from u in seu.DefaultIfEmpty()
                    select new { se, c, u, es };
        //2. filter
        if (request.ExamScheduleID != null && request.ExamScheduleID != 0)
        {
            query = query.Where(p => p.se.ExamScheduleID == request.ExamScheduleID);
        }

        if (request.UserId != null)
        {
            query = query.Where(x => x.c.ClassDetails.Any(x => x.UserID == request.UserId && x.IsTeacher == Data.Enums.Teacher.Teacher));
        }

        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new StudentExamsViewModel()
            {
                StudentID = x.u.Id,
                FirstName = x.u.FirstName,
                LastName = x.u.LastName,
                ClassID = x.c.ID,
                ClassName = x.c.ClassName,
                StudentExamID = x.se.StudentExamID,
                ExamScheduleID = x.es.ExamScheduleID,
                ExamScheduleName = x.es.ExamScheduleName,
                Mark = x.se.Mark,
                Note = x.se.Note,
                DateTimeStudentExam = x.se.StudentExamDateTime
            }).ToListAsync();

        //4. Select and projection
        var pagedResult = new PagedResult<StudentExamsViewModel>()
        {
            TotalRecords = totalRow,
            PageSize = request.PageSize,
            PageIndex = request.PageIndex,
            Items = data
        };
        return pagedResult;
    }

    public async Task<PagedResult<StudentExamsViewModel>> GetAllAdmin(GetManageStudentExamPagingRequest request)
    {
        //1. Select join
        var query = from se in _context.StudentExams
                    join es in _context.ExamSchedules on se.ExamScheduleID equals es.ExamScheduleID
                    join c in _context.Classes.Include(x=> x.ClassDetails) on es.ClassID equals c.ID
                    join u in _userManager.Users on se.StudentID equals u.Id into seu
                    from u in seu.DefaultIfEmpty()
                    select new { se, c, u, es };
        //2. filter
        if (request.ExamScheduleID != null && request.ExamScheduleID != 0)
        {
            query = query.Where(p => p.se.ExamScheduleID == request.ExamScheduleID);
        }

        if (request.UserId != null)
        {
            query = query.Where(x => x.c.ClassDetails.Any(x => x.UserID == request.UserId && x.IsTeacher == Data.Enums.Teacher.Teacher));
        }

        //3. Paging
        int totalRow = await query.CountAsync();

        var data = await query
            .Select(x => new StudentExamsViewModel()
            {
                StudentID = x.u.Id,
                FirstName = x.u.FirstName,
                LastName = x.u.LastName,
                ClassID = x.c.ID,
                ClassName = x.c.ClassName,
                StudentExamID = x.se.StudentExamID,
                ExamScheduleID = x.es.ExamScheduleID,
                ExamScheduleName = x.es.ExamScheduleName,
                Mark = x.se.Mark,
                Note = x.se.Note,
                DateTimeStudentExam = x.se.StudentExamDateTime
            }).ToListAsync();

        //4. Select and projection
        var pagedResult = new PagedResult<StudentExamsViewModel>()
        {
            TotalRecords = totalRow,
            PageSize = request.PageSize,
            PageIndex = request.PageIndex,
            Items = data
        };
        return pagedResult;
    }

    public async Task<int> Update(StudentExamsUpdateRequest request)
    {
        var studentExam = await _context.StudentExams.FindAsync(request.StudentExamID);
        if (studentExam == null) throw new ClassroomException($"Cannot find a student exam {request.StudentExamID}");
        studentExam.Mark = request.Mark;
        studentExam.ExamScheduleID = request.ExamScheduleID;
        return await _context.SaveChangesAsync();
    }

    public async Task<StudentExamsViewModel> GetById(int ExamScheduleID, string UserId)
    {
        var studentExam = await _context.StudentExams.FirstOrDefaultAsync(x => x.ExamScheduleID == ExamScheduleID && x.StudentID == UserId);
        if (studentExam == null) return null;

        var student = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == studentExam.StudentID);

        var studentExamViewModel = new StudentExamsViewModel()
        {
            StudentExamID = studentExam.StudentExamID,
            ExamScheduleID = studentExam.ExamScheduleID,
            StudentID = student.Id,
            Mark = studentExam.Mark,
            Note = studentExam.Note,
            DateTimeStudentExam = studentExam.StudentExamDateTime
        };
        return studentExamViewModel;
    }
}