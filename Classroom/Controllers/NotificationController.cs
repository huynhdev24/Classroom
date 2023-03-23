using AutoMapper;
using Classroom.Application.Catalog.Notifications;
using Classroom.Models.Catalog.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace Classroom.Controllers;

public class NotificationController : BaseController
{
    private readonly INotificationService _notificationService;
    private readonly INotificationService _classService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public NotificationController(INotificationService notificationService,
                                  INotificationService classService,
                                  IMapper mapper,
                                  IConfiguration configuration)
    {
        _configuration = configuration;
        _notificationService = notificationService;
        _classService = classService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
    {
        var request = new GetManageNotificationPagingRequest()
        {
            Keyword = keyword,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        var data = await _notificationService.GetAllPaging(request);
        ViewBag.Keyword = keyword;
        if (TempData["result"] != null)
        {
            ViewBag.SuccessMsg = TempData["result"];
        }
        return View(data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] NotificationCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            TempData["result"] = "Vui lòng nhập đầy đủ cho thông báo";
            return Redirect(request.ReturnUrl);
        }

        var result = await _notificationService.Create(request);
        if (result != 0)
        {
            TempData["result"] = "Thêm mới thông báo thành công";
            if (request.ReturnUrl != null)
                return Redirect(request.ReturnUrl);
            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", "Thêm thông báo thất bại");
        return Redirect(request.ReturnUrl);
    }

    [HttpGet("chinh-sua-thong-bao")]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _notificationService.GetById(id);
        if (result == null) return NotFound();
        var notificationViewModel = _mapper.Map<NotificationViewModel, NotificationUpdateRequest>(result);
        return View(notificationViewModel);
    }

    [HttpPost("chinh-sua-thong-bao")]
    [ValidateAntiForgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Edit([FromForm] NotificationUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var result = await _notificationService.Update(request);
        TempData["result"] = "Cập nhật thông báo thành công";
        var notification = await _notificationService.GetById(request.NotificationID);
        return RedirectToAction("Details", "Class", new { id = notification.ID });
    }

    public async Task<IActionResult> Delete(int id, string returnUrl)
    {
        if (!ModelState.IsValid)
            return View();

        var result = await _notificationService.Delete(id);
        if (result != 0)
        {
            TempData["result"] = "Xoá thông báo thành công";
            return Redirect(returnUrl);
        }

        ModelState.AddModelError("", "Xoá thông báo thất bại");
        return Redirect(returnUrl);
    }
}