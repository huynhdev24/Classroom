namespace Classroom.Models.Common;

/// <summary>
/// PagedResultBase
/// </summary>
/// <author>huynhdev24</author>
public class PagedResultBase
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public int PageCount
    {
        get
        {
            var pageCount = (double)TotalRecords / PageSize;
            return (int)Math.Ceiling(pageCount);
        }
    }
}