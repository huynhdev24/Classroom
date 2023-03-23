using Classroom.Models.Common;

namespace Classroom.Models.Catalog.Classes;

public class GetAllStudentInClassPagingRequest : PagingRequestBase
{
    public int? ClassID { get; set; }
}