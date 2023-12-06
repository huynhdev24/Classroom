using Classroom.Models.Catalog.Comments;
using Classroom.Models.Common;

namespace Classroom.Application.Catalog.Comments;

/// <summary>
/// ICommentService
/// </summary>
public interface ICommentService
{
    Task<int> Create(CommentCreateRequest request);
    Task<int> Update(CommentUpdateRequest request);
    Task<int> Delete(int CommentID);
    Task<CommentViewModel> GetById(int NotificationID);
    Task<PagedResult<CommentViewModel>> GetAllPaging(GetManageCommentPagingRequest request);
}