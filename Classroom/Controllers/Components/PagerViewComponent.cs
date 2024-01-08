using Classroom.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace Classroom.Controllers.Components;

public class PagerViewComponent : ViewComponent
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    public Task<IViewComponentResult> InvokeAsync(PagedResultBase result)
    {
        return Task.FromResult((IViewComponentResult)View("Default", result));
    }
}