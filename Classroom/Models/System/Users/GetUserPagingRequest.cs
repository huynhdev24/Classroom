using Classroom.Models.Common;

namespace Classroom.Models.System.Users
{
    public class GetUserPagingRequest : PagingRequestBase
    {
        public string? Keyword { get; set; }
    }
}