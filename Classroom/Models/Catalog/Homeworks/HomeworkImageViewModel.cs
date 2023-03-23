namespace Classroom.Models.Catalog.Homeworks;

public class HomeworkImageViewModel
{
    public int? HomeworkID { set; get; }

    public int? ImageID { set; get; }

    public string? ImagePath { get; set; }

    public bool? IsDefault { get; set; }
}

