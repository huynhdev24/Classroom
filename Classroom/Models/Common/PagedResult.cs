namespace Classroom.Models.Common;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <author>huynhdev24</author>
public class PagedResult<T> : PagedResultBase
{
    public IEnumerable<T>? Items { set; get; }
}