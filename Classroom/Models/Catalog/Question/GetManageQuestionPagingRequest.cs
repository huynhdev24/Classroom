using Classroom.Models.Common;

namespace Classroom.Models.Catalog.Question;

public class GetManageQuestionPagingRequest : PagingRequestBase
{
    public string? Keyword { set; get; }

    public int ExamScheduleID { set; get; }
}