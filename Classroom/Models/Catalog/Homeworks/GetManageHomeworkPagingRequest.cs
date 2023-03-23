using Classroom.Models.Common;

namespace Classroom.Models.Catalog.Homeworks;

public class GetManageHomeworkPagingRequest : PagingRequestBase
{
    public string? Keyword { get; set; }
    public int ClassID { get; set; }
    public string? UserId { get; set; }
}