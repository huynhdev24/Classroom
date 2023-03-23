using Classroom.Models.Common;

namespace Classroom.Models.Catalog.Classes;

public class ClassPagingRequest : PagingRequestBase
{
    public string? Keyword { get; set; }
}