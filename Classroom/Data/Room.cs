namespace Classroom.Data;

/// <summary>
/// Room
/// </summary>
public class Room
{
    public int Id { set; get; }
    public ApplicationUser? Admin { set; get; }
    public string? Name { set; get; }
    public ICollection<Message>? Messages { set; get; }
}

