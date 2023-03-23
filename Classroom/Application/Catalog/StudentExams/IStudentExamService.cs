using Classroom.Models.Catalog.StudentExams;
using Classroom.Models.Common;

namespace Classroom.Application.Catalog.StudentExams;

public interface IStudentExamService
{
    Task<int> Create(StudentExamsCreateRequest request);
    Task<int> Update(StudentExamsUpdateRequest request);
    Task<int> Delete(int StudentExamID);
    Task<StudentExamsViewModel> GetById(int ExamScheduleID, string UserId);
    Task<PagedResult<StudentExamsViewModel>> GetAllPaging(GetManageStudentExamPagingRequest request);
    Task<PagedResult<StudentExamsViewModel>> GetAllAdminPaging(GetManageStudentExamPagingRequest request);
    Task<PagedResult<StudentExamsViewModel>> GetAllAdmin(GetManageStudentExamPagingRequest request);
}