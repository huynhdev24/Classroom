using Classroom.Data.Enums;
using Classroom.Models.Common;

namespace Classroom.Models.Catalog.Submissions;

public class GetManageSubmissionPagingRequest : PagingRequestBase
{
    public string? Keyword { get; set; }
    public int HomeworkID { get; set; }
    public string? UserId { get; set; }
    
}