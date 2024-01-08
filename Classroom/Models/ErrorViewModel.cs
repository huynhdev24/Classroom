namespace Classroom.Models
{
    /// <summary>
    /// ErrorViewModel
    /// </summary>
    /// <author>huynhdev24</author>
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}