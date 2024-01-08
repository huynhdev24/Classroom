using AutoMapper;
using Classroom.Application.Catalog.ExamSchedules;
using Classroom.Application.Catalog.Questions;
using Classroom.Application.Catalog.StudentExams;
using Classroom.Data;
using Classroom.Models.Catalog.Question;
using Classroom.Models.Catalog.StudentExams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace Classroom.Controllers;

public class QuestionController : BaseController
{
    private readonly IQuestionService _questionService;
    private readonly IExamScheduleService _examScheduleService;
    private readonly IStudentExamService _studentExamService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="questionService"></param>
    /// <param name="examScheduleService"></param>
    /// <param name="studentExamService"></param>
    /// <param name="configuration"></param>
    /// <param name="mapper"></param>
    /// <author>huynhdev24</author>
    public QuestionController(IQuestionService questionService,
                              IExamScheduleService examScheduleService,
                              IStudentExamService studentExamService,
                              IConfiguration configuration,
                              IMapper mapper)
    {
        _questionService = questionService;
        _examScheduleService = examScheduleService;
        _studentExamService = studentExamService;
        _configuration = configuration;
        _mapper = mapper;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="exId"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpGet]
    public async Task<JsonResult> LoadData(int page, int pageSize = 10, int exId = 0)
    {
        var request = new GetManageQuestionPagingRequest()
        {
            ExamScheduleID = exId,
            Keyword = null,
            PageIndex = page,
            PageSize = pageSize
        };
        var data = await _questionService.GetAllPaging(request);
        int totalRow = data.TotalRecords;
        return Json(new
        {
            data = data.Items,
            total = totalRow,
            status = true
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpGet]
    public async Task<JsonResult> LoadExam(int id)
    {
        var examPaper = await _questionService.GetExamPaper(id);
        return Json(new
        {
            data = examPaper.Items,
            status = true
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpPost]
    public async Task<JsonResult> UpdatePoint(string model)
    {
        Question question = JsonConvert.DeserializeObject<Question>(model);

        //save db
        var entity = await _questionService.GetById(question.QuestionID);
        entity.QuestionString = question.QuestionString;
        entity.Point = question.Point;
        entity.Option1 = question.Option1;
        entity.Option2 = question.Option2;
        entity.Option3 = question.Option3;
        entity.Option4 = question.Option4;
        entity.OptionCorrect = question.OptionCorrect;

        await _questionService.Update(_mapper.Map<QuestionViewModel, QuestionUpdateRequest>(entity));
        return Json(new
        {
            status = true
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpPost]
    public async Task<JsonResult> DeleteQuestion(int id)
    {
        var Question = await _questionService.GetById(id);
        var result = await _questionService.Delete(id);
        return Json(new
        {
            status = true
        });

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpPost]
    public async Task<JsonResult> UpdateAll(string model)
    {
        List<Question> questions = JsonConvert.DeserializeObject<List<Question>>(model);

        foreach (var item in questions)
        {
            var entity = await _questionService.GetById(item.QuestionID);
            entity.QuestionString = item.QuestionString;
            entity.Point = item.Point;
            entity.Option1 = item.Option1;
            entity.Option2 = item.Option2;
            entity.Option3 = item.Option3;
            entity.Option4 = item.Option4;
            entity.OptionCorrect = item.OptionCorrect;

            await _questionService.Update(_mapper.Map<QuestionViewModel, QuestionUpdateRequest>(entity));
        }

        //save db

        return Json(new
        {
            status = true
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="strQuestionsCreateRequest"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    public async Task<JsonResult> SaveData(string strQuestionsCreateRequest)
    {
        QuestionsCreateRequest question = JsonConvert.DeserializeObject<QuestionsCreateRequest>(strQuestionsCreateRequest);
        bool status = false;
        string message = string.Empty;

        var result = await _questionService.Create(question);
        if (result != null)
        {
            status = true;
        }
        else
        {
            status = false;
            message = "Thêm câu hỏi thất bại";
        }

        return Json(new
        {
            status = status,
            message = message
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [Route("quan-ly-cau-hoi")]
    public async Task<IActionResult> Index()
    {
        // danh sách kỳ thi thuộc lớp học của mình
        var list = await _examScheduleService.GetAllMyExamSchedules(User.Identity.Name);
        ViewData["ExamScheduleID"] = new SelectList(list, "ExamScheduleID", "ExamScheduleName");
        if (TempData["result"] != null)
        {
            ViewBag.SuccessMsg = TempData["result"];
        }
        return View();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpGet("bai-thi")]
    public async Task<IActionResult> Exam(int id)
    {
        if (id == 0) return NotFound();
        var exS = await _examScheduleService.GetById(id);
        HttpContext.Session.SetString("DateTimeStudentExam", DateTime.Now.ToString());
        HttpContext.Session.SetString("DateTimeStudentExamD", DateTime.Now.ToString("HH:mm - dddd dd/MM/yyyy"));
        HttpContext.Session.SetString("ExamScheduleID", id.ToString());
        return View(exS);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mark"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpPost]
    public async Task<IActionResult> SaveResult(int mark)
    {
        var exId = Convert.ToInt32(HttpContext.Session.GetString("ExamScheduleID"));
        var dtEx = Convert.ToDateTime(HttpContext.Session.GetString("DateTimeStudentExam"));

        StudentExamsCreateRequest secr = new StudentExamsCreateRequest()
        {
            ExamScheduleID = exId,
            UserName = User.Identity.Name,
            Mark = mark,
            DateTimeStudentExam = dtEx
        };

        await _studentExamService.Create(secr);

        return Json(new
        {
            data = exId,
            status = true
        });
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
    public async Task<IActionResult> Create([FromForm] QuestionsCreateRequest request)
    {

        var result = await _questionService.Create(request);
        if (result != null)
        {
            TempData["result"] = "Thêm mới bài tập thành công";
            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", "Thêm bài tập thất bại");
        return View(request);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <param name="ExamScheduleId"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    public async Task<IActionResult> Import(IFormFile file, int ExamScheduleId)
    {
        if(file == null) {
            TempData["result"] = $"Vui lòng chọn file";
            return RedirectToAction("Index", "Question");
        }
        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            using (var package = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;
                for (int row = 3; row <= rowCount; row++)
                {
                    try
                    {
                        var question = new QuestionsCreateRequest
                        {
                            ExamScheduleID = ExamScheduleId,
                            QuestionString = worksheet.Cells[row, 2].Value.ToString().Trim(),
                            Point = Int32.Parse(worksheet.Cells[row, 3].Value.ToString().Trim()),
                            Option1 = worksheet.Cells[row, 4].Value.ToString().Trim(),
                            Option2 = worksheet.Cells[row, 5].Value.ToString().Trim(),
                            Option3 = worksheet.Cells[row, 6].Value.ToString().Trim(),
                            Option4 = worksheet.Cells[row, 7].Value.ToString().Trim(),
                            OptionCorrect = Int32.Parse(worksheet.Cells[row, 8].Value.ToString().Trim()),
                        };
                        await _questionService.Create(question);
                    }catch{
                        
                        break;
                    }
                }
                TempData["result"] = $"Nhập file câu hỏi thành công";
            }
        }

        return RedirectToAction("Index", "Question");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var result = await _questionService.GetById(id);
        if (result == null) NotFound();
        if (TempData["result"] != null)
        {
            ViewBag.SuccessMsg = TempData["result"];
        }
        return View(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _questionService.GetById(id);
        if (result == null) return NotFound();
        var QuestionViewModel = _mapper.Map<QuestionViewModel, QuestionUpdateRequest>(result);
        return View(QuestionViewModel);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <author>huynhdev24</author>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Edit([FromForm] QuestionUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var result = await _questionService.Update(request);
        TempData["result"] = "Cập nhật bài tập thành công";
        var Question = await _questionService.GetById(request.QuestionID);
        return RedirectToAction("Details", "Question", new { id = Question.QuestionID });
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
        var Question = await _questionService.GetById(id);
        var result = await _questionService.Delete(id);
        if (result != 0)
        {
            TempData["result"] = "Xoá bài tập thành công";
            return RedirectToAction("Details", "Class", new { id = Question.QuestionID });
        }

        ModelState.AddModelError("", "Xoá bài tập thất bại");
        return Redirect(returnUrl);
    }
}