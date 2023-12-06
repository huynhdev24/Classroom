using Classroom.Models.Common;

namespace Classroom.Models.System.Users
{
    /// <summary>
    /// GetUserPagingRequest
    /// </summary>
    public class GetUserPagingRequest : PagingRequestBase
    {
        public string? Keyword { get; set; }
    }
}