using System.Security.Cryptography.X509Certificates;
using Classroom.Utilities.Exceptions;
using Classroom.Models.Common;
using Classroom.Models.Catalog.Question;
using Microsoft.EntityFrameworkCore;
using Classroom.Application.Common;
using System.Net.Http.Headers;
using Classroom.Data;
using AutoMapper;

namespace Classroom.Application.Catalog.Questions;

public class QuestionService : IQuestionService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private const string USER_CONTENT_FOLDER_NAME = "user-content";

    public QuestionService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Create(QuestionsCreateRequest request)
    {
        var question = _mapper.Map<QuestionsCreateRequest, Question>(request);
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
        return question.QuestionID;
    }

    public async Task<int> Delete(int QuestionID)
    {
        var question = await _context.Questions.FindAsync(QuestionID);
        if (question == null) throw new ClassroomException($"Cannot find a question {QuestionID}");

        _context.Questions.Remove(question);
        return await _context.SaveChangesAsync();
    }

    public async Task<PagedResult<Question>> GetExamPaper(int ExamScheduleID)
    {
        var data = await _context.Questions.Where(x=> x.ExamScheduleID == ExamScheduleID).ToListAsync();
        var pagedResult = new PagedResult<Question>()
        {
            Items = data
        };
        return pagedResult;
    }

    public async Task<PagedResult<QuestionViewModel>> GetAllPaging(GetManageQuestionPagingRequest request)
    {
        //1. Select join
        var query = from q in _context.Questions
                    join e in _context.ExamSchedules on q.ExamScheduleID equals e.ExamScheduleID into qn
                    from e in qn.DefaultIfEmpty()
                    select new { q, e };
        //2. filter
        if (request.ExamScheduleID != null && request.ExamScheduleID != 0)
        {
            query = query.Where(p => p.q.ExamScheduleID == request.ExamScheduleID);
        }

        //3. Paging
        int totalRow = await query.CountAsync();
        var data = await query.OrderByDescending(x=> x.q.QuestionID).Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new QuestionViewModel()
            {
               QuestionID = x.q.QuestionID,
               ExamScheduleID = x.e.ExamScheduleID,
               QuestionString = x.q.QuestionString,
               Point = x.q.Point,
               Option1 = x.q.Option1,
               Option2 = x.q.Option2,
               Option3 = x.q.Option3,
               Option4 = x.q.Option4,
               OptionCorrect = x.q.OptionCorrect,
            }).ToListAsync();

        //4. Select and projection
        var pagedResult = new PagedResult<QuestionViewModel>()
        {
            TotalRecords = totalRow,
            PageSize = request.PageSize,
            PageIndex = request.PageIndex,
            Items = data
        };
        return pagedResult;
    }

    public async Task<QuestionViewModel> GetById(int QuestionID)
    {
        var question = await _context.Questions.FindAsync(QuestionID);
        if (question == null) throw new ClassroomException($"Cannot find a question {QuestionID}");

        var questionViewModel = _mapper.Map<Question, QuestionViewModel>(question);
        return questionViewModel;
    }

    public async Task<int> Update(QuestionUpdateRequest request)
    {
        var question = await _context.Questions.FindAsync(request.QuestionID);
        if (question == null) throw new ClassroomException($"Cannot find a question {request.QuestionID}");
        question.QuestionString = request.QuestionString;
        question.Point = request.Point;
        question.Option1 = request.Option1;
        question.Option2 = request.Option2;
        question.Option3 = request.Option3;
        question.Option4 = request.Option4;
        question.OptionCorrect = request.OptionCorrect;
        return await _context.SaveChangesAsync();
    }
}