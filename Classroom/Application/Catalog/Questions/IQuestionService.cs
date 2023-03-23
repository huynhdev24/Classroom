using Classroom.Data;
using Classroom.Models.Catalog.Question;
using Classroom.Models.Common;

namespace Classroom.Application.Catalog.Questions;
public interface IQuestionService
{
    Task<int> Create(QuestionsCreateRequest request);
    Task<int> Update(QuestionUpdateRequest request);
    Task<int> Delete(int QuestionID);
    Task<QuestionViewModel> GetById(int QuestionID);
    Task<PagedResult<Question>> GetExamPaper(int ExamScheduleID);
    Task<PagedResult<QuestionViewModel>> GetAllPaging(GetManageQuestionPagingRequest request);
}