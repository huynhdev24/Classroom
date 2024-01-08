using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Classroom.Controllers;
public class BaseController : Controller
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <author>huynhdev24</author>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var sessions = context.HttpContext.Session.GetString("Session");
        if (sessions == null)
        {
            context.Result = new RedirectResult("/Identity/Account/Login");
        }
        base.OnActionExecuting(context);
    }
}