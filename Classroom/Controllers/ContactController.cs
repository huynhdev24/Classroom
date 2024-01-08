using AutoMapper;
using Classroom.Application.Catalog.Contacts;
using Classroom.Data;
using Classroom.Models.Catalog.Contact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Classroom.Controllers;

public class ContactController : Controller
{
    private readonly IContactService _ContactService;
    private readonly IContactService _classService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ContactService"></param>
    /// <param name="classService"></param>
    /// <param name="mapper"></param>
    /// <param name="configuration"></param>
    /// <author>huynhdev24</author>
    public ContactController(IContactService ContactService,
                                  IContactService classService,
                                  IMapper mapper,
                                  IConfiguration configuration)
    {
        _configuration = configuration;
        _ContactService = ContactService;
        _classService = classService;
        _mapper = mapper;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [Authorize(Policy = "RequireAdmin")]
    [HttpGet("admin/danh-sach-phan-hoi")]
    public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
    {
        var request = new GetAllPagingRequest()
        {
            Keyword = keyword,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        var data = await _ContactService.GetAllPaging(request);
        ViewBag.Keyword = keyword;
        if (TempData["result"] != null)
        {
            ViewBag.SuccessMsg = TempData["result"];
        }
        return View(data);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpPost("phan-hoi")]
    public async Task<IActionResult> Create([FromForm] Contact request)
    {
        if (!ModelState.IsValid)
        {
            TempData["result"] = "Vui lòng nhập đầy đủ cho thông báo";
            return RedirectToAction("Index", "Home");
        }

        var result = await _ContactService.Create(request);
        if (result == true)
        {
            TempData["result"] = "Chúng tôi đã nhận được phản hồi của bạn, chúng tôi sẽ liên hệ với bạn trong thời gian gần nhất";
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Thêm phản hồi thất bại, vui lòng kiểm tra lại thông tin");
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ContactID"></param>
    /// <param name="Email"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [Authorize(Policy = "RequireAdmin")]
    [HttpPost("gui-mail")]
    public IActionResult Send(int ContactID, string Email, string message)
    {
        Classroom.Utilities.Helpers.SendMail.SendEmail(Email, "Phản hồi từ Classroom", message, "");

        TempData["result"] = "Gửi email phản hồi đến người dùng thành công";
        return RedirectToAction("Details", "Contact", new { id = ContactID });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [Authorize(Policy = "RequireAdmin")]
    [HttpGet("admin/thong-tin-phan-hoi")]
    public async Task<IActionResult> Details(int id)
    {
        if (!ModelState.IsValid)
            return View();
        if (TempData["result"] != null)
        {
            ViewBag.SuccessMsg = TempData["result"];
        }
        var result = await _ContactService.GetById(id);
        return View(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!ModelState.IsValid)
            return View();

        var result = await _ContactService.Delete(id);
        if (result == true)
        {
            TempData["result"] = "Xoá phản hồi thành công";
            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", "Xoá phản hồi thất bại");
        return RedirectToAction("Index");
    }
}