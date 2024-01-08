using Classroom.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Classroom.Core.ViewModels
{
    /// <summary>
    /// EditUserViewModel
    /// </summary>
    /// <author>huynhdev24</author>
    public class EditUserViewModel
    {
        public ApplicationUser User { get; set; }

        public IList<SelectListItem> Roles { get; set; }
    }
}
