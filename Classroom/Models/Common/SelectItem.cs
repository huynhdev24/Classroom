namespace Classroom.Models.Common;

/// <summary>
/// SelectItem
/// </summary>
public class SelectItem
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public bool Selected { get; set; }
    public object Select()
    {
        throw new NotImplementedException();
    }
}