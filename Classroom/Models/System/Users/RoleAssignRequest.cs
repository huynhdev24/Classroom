using Classroom.Models.Common;

namespace Classroom.Models.System.Users;
public class RoleAssignRequest
{
    public Guid Id { get; set; }
    public List<SelectItem> Roles { get; set; } = new List<SelectItem>();
}