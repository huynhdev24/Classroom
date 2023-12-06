using Classroom.Models.Common;

namespace Classroom.Models.System.Users;

/// <summary>
/// RoleAssignRequest
/// </summary>
public class RoleAssignRequest
{
    public Guid Id { get; set; }
    public List<SelectItem> Roles { get; set; } = new List<SelectItem>();
}