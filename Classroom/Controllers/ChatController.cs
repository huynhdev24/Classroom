using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Classroom.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        [Route("tro-chuyen")]
        public IActionResult Index()
        {
            return View();
        }

        public void OnGet()
        {

        }
    }
}