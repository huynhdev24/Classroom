using Classroom.Application.Catalog.Comments;
using Classroom.Models.Catalog.Comments;
using Microsoft.AspNetCore.Mvc;

namespace Classroom.Controllers;

public class CommentController : BaseController
{
    private readonly ICommentService _commentService;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="commentService"></param>
    /// <param name="configuration"></param>
    /// <author>huynhdev24</author>
    public CommentController(ICommentService commentService,
        IConfiguration configuration)
    {
        _configuration = configuration;
        _commentService = commentService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpGet]
    public IActionResult Create()
    {
        if (User.Identity != null)
        {
            var user = User.Identity.Name;
        }
        return View();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
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