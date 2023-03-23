namespace Classroom.Models.Common;

public class PagedResult<T> : PagedResultBase
{
    public IEnumerable<T>? Items { set; get; }
}