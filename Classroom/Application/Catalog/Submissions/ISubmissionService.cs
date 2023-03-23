using Classroom.Models.Catalog.Submissions;
using Classroom.Models.Common;

namespace Classroom.Application.Catalog.Submissions;
public interface ISubmissionService
{
    Task<bool> Create(SubmissionCreateRequest request);
    Task<bool> Update(SubmissionUpdateRequest request);
    Task<bool> UpdateMark(SubmissionUpdateMarkRequest request);
    Task<bool> Delete(int SubmissionID);
    Task<SubmissionViewModel> GetById(int SubmissionID);
    Task<PagedResult<SubmissionViewModel>> GetAllPaging(GetManageSubmissionPagingRequest request);
    Task<PagedResult<SubmissionViewModel>> GetMyAllPaging(GetManageSubmissionPagingRequest request);
    Task<PagedResult<SubmissionViewModel>> GetMyAll(GetManageSubmissionPagingRequest request);
}