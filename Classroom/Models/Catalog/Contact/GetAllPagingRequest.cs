using Classroom.Models.Common;

namespace Classroom.Models.Catalog.Contact;

public class GetAllPagingRequest : PagingRequestBase
{
    public string? Keyword { set; get; }
}