using System.ComponentModel.DataAnnotations;

namespace Classroom.Models.Catalog.Comments;

public class CommentUpdateRequest
{
    public int CommentID { set; get; }

    [Display(Name = "Nội dung")]
    public string? Content { set; get; }

    public ICollection<IFormFile>? CommentImages { get; set; }
}
