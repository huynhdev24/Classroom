using Classroom.Models.Common;

namespace Classroom.Models.Catalog.Comments;

public class GetManageCommentPagingRequest : PagingRequestBase
{
    public string? Keyword { get; set; }
    public int NotificationID { get; set; }
}