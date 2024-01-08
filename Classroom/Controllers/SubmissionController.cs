using AutoMapper;
using ClosedXML.Excel;
using Classroom.Application.Catalog.Homeworks;
using Classroom.Application.Catalog.Submissions;
using Classroom.Models.Catalog.Submissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Classroom.Controllers;

public class SubmissionController : BaseController
{
    private readonly ISubmissionService _submissionService;
    private readonly IHomeworkService _homeworkService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="submissionService"></param>
    /// <param name="homeworkService"></param>
    /// <param name="mapper"></param>
    /// <param name="configuration"></param>
    /// <author>huynhdev24</author>
    public SubmissionController(ISubmissionService submissionService,
                                IHomeworkService homeworkService,
                                IMapper mapper,
                                IConfiguration configuration)
    {
        _configuration = configuration;
        _submissionService = submissionService;
        _homeworkService = homeworkService;
        _mapper = mapper;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="HomeworkID"></param>
    /// <param name="keyword"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpGet("giao-vien/danh-sach-nop-bai")]
    public async Task<IActionResult> Index(int HomeworkID, string keyword, int pageIndex = 1, int pageSize = 10)
    {
        var request = new GetManageSubmissionPagingRequest()
        {
            HomeworkID = HomeworkID,
            Keyword = keyword,
            UserId = HttpContext.Session.GetString("UserId"),
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        var data = await _submissionService.GetMyAllPaging(request);
        ViewBag.Keyword = keyword;
        ViewBag.HomeworkID = HomeworkID;

        var list = await _homeworkService.GetAllMyAdminHomework(HttpContext.Session.GetString("UserId"));
        ViewBag.listH = list;

        if (TempData["result"] != null)
        {
            ViewBag.SuccessMsg = TempData["result"];
        }
        return View(data);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="HomeworkID"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    public async Task<IActionResult> Report(int HomeworkID, string keyword)
    {
        var request = new GetManageSubmissionPagingRequest()
        {
            HomeworkID = HomeworkID,
            Keyword = keyword,
            UserId = HttpContext.Session.GetString("UserId")
        };
        var data = await _submissionService.GetMyAll(request);
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Submissions");
            worksheet.Cell(1, 1).Value = "Tên lớp học: " + data.Items.ToList()[0].ClassName;
            worksheet.Cell(2, 1).Value = "Tên bài tập: " + data.Items.ToList()[0].HomeworkName;
            var currentRow = 3;
            worksheet.Cell(currentRow, 1).Value = "STT";
            worksheet.Cell(currentRow, 2).Value = "Họ";
            worksheet.Cell(currentRow, 3).Value = "Tên";
            worksheet.Cell(currentRow, 4).Value = "Thời gian nộp bài";
            worksheet.Cell(currentRow, 5).Value = "Thời gian cập nhật lại bài nộp bài";
            worksheet.Cell(currentRow, 6).Value = "Trạng thái";
            worksheet.Cell(currentRow, 7).Value = "Điểm";

            foreach (var submission in data.Items)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = currentRow - 3;
                worksheet.Cell(currentRow, 2).Value = submission.FirstName;
                worksheet.Cell(currentRow, 3).Value = submission.LastName;
                worksheet.Cell(currentRow, 4).Value = submission.SubmissionDateTime;
                worksheet.Cell(currentRow, 5).Value = submission.DateTimeUpdated;

                if (submission.DateTimeUpdated < submission.Deadline)
                    worksheet.Cell(currentRow, 6).Value = "Nộp đúng hạn";
                else
                    worksheet.Cell(currentRow, 6).Value = "Nộp trễ";
                worksheet.Cell(currentRow, 7).Value = submission.Mark;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + " Bảng điểm "+data.Items.ToList()[0].HomeworkName;
                return File(
                content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName + ".xlsx"
                );
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bai_tap"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpGet("danh-sach-nop-bai")]
    public async Task<IActionResult> ListSubmission(int bai_tap, int pageIndex = 1, int pageSize = 10)
    {
        var request = new GetManageSubmissionPagingRequest()
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            HomeworkID = bai_tap
        };
        var data = await _submissionService.GetAllPaging(request);
        ViewBag.Keyword = bai_tap;

        var homeworks = await _homeworkService.GetAll();
        ViewBag.Homeworks = homeworks.Select(x => new SelectListItem()
        {
            Text = x.HomeworkName + " - " + x.ClassName,
            Value = x.HomeworkID.ToString()
        });
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
    public async Task<IActionResult> Create([FromForm] SubmissionCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            TempData["result"] = "Vui lòng nhập đầy đủ cho thông báo";
            return Redirect(request.ReturnUrl);
        }

        var result = await _submissionService.Create(request);
        if (result == true)
        {
            TempData["result"] = "Nộp bài thành công";
            if (request.ReturnUrl != null)
                return Redirect(request.ReturnUrl);
            return RedirectToAction(request.ReturnUrl);
        }
        TempData["result"] = "Nộp bài thất bại, bạn đã nộp bài rồi, chỉ có thể chỉnh sửa";
        return Redirect(request.ReturnUrl);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpGet("nop-bai")]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _submissionService.GetById(id);
        if (result == null) return NotFound();
        var submissionViewModel = _mapper.Map<SubmissionViewModel, SubmissionUpdateRequest>(result);
        return View(submissionViewModel);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpPost("nop-bai")]
    [ValidateAntiForgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Edit([FromForm] SubmissionUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var result = await _submissionService.Update(request);
        TempData["result"] = "Cập nhật bài nộp thành công";
        var submission = await _submissionService.GetById(request.SubmissionID);
        return RedirectToAction("Details", "Homework", new { id = submission.HomeworkID });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpPost("cham-diem")]
    public async Task<IActionResult> UpdateMark([FromForm] SubmissionUpdateMarkRequest request)
    {
        if (request.Mark == null || request.Mark < 0)
            return RedirectToAction("Details", "Submission", new { id = request.SubmissionID });
        await _submissionService.UpdateMark(request);
        TempData["result"] = "Cập nhật điểm thành công";
        return RedirectToAction("Details", "Submission", new { id = request.SubmissionID });
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
        var submission = await _submissionService.GetById(id);
        var result = await _submissionService.Delete(id);
        if (result == true)
        {
            TempData["result"] = "Xoá bài nộp thành công";
            return RedirectToAction("Details", "Homework", new { id = submission.HomeworkID });
        }

        ModelState.AddModelError("", "Xoá bài nộp thất bại");
        return Redirect(returnUrl);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpGet("chi-tiet-bai-nop")]
    public async Task<IActionResult> Details(int id)
    {
        var result = await _submissionService.GetById(id);
        if (result == null) NotFound();
        if (TempData["result"] != null)
        {
            ViewBag.SuccessMsg = TempData["result"];
        }
        return View(result);
    }
}