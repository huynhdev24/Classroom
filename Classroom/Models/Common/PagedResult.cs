namespace Classroom.Models.Common;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class PagedResult<T> : PagedResultBase
{
    public IEnumerable<T>? Items { set; get; }
}