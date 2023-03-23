using Classroom.Application.Catalog.Comments;
using Classroom.Models.Catalog.Comments;
using Microsoft.AspNetCore.Mvc;

namespace Classroom.Controllers;

public class CommentController : BaseController
{
    private readonly ICommentService _commentService;
    private readonly IConfiguration _configuration;

    public CommentController(ICommentService commentService,
        IConfiguration configuration)
    {
        _configuration = configuration;
        _commentService = commentService;
    }

    public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
    {
        var request = new GetManageCommentPagingRequest()
        {
            Keyword = keyword,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        var data = await _commentService.GetAllPaging(request);
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
        if (User.Identity != null)
        {
            var user = User.Identity.Name;
        }
        return View();
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CommentCreateRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);


        var result = await _commentService.Create(request);
        if (result != 0)
        {
            TempData["result"] = "Bạn vừa bình luận vào bài viết";
            if (request.ReturnUrl != null)
                return Redirect(request.ReturnUrl);
            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", "Bình luận thất bại");
        return View(request);
    }

    public async Task<IActionResult> Delete(int id, string returnUrl)
    {
        if (!ModelState.IsValid)
            return View();

        var result = await _commentService.Delete(id);
        if (result != 0)
        {
            TempData["result"] = "Đã xoá bình luận";
            return Redirect(returnUrl);
        }

        ModelState.AddModelError("", "Xoá bình luận thất bại");
        return Redirect(returnUrl);
    }
}