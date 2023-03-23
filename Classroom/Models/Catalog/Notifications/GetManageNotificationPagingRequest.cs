using Classroom.Models.Common;

namespace Classroom.Models.Catalog.Notifications;

public class GetManageNotificationPagingRequest : PagingRequestBase
{
    public string? Keyword { get; set; }
    public int ClassID { get; set; }
}