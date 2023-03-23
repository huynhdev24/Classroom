using Classroom.Application.Catalog.ExamSchedules;
using Classroom.Models.Catalog.ExamSchedules;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Classroom.Application.Catalog.StudentExams;
using Classroom.Application.Catalog.Classes;
using Classroom.Models.Catalog.StudentExams;
using ClosedXML.Excel;

namespace Classroom.Controllers
{
    public class ExamScheduleController : BaseController
    {
        private readonly IExamScheduleService _examScheduleService;
        private readonly IStudentExamService _studentExamService;
        private readonly IClassService _classService;
        private readonly IMapper _mapper;

        public ExamScheduleController(IExamScheduleService examScheduleService,
                                      IStudentExamService studentExamService,
                                      IClassService classService,
                                      IMapper mapper)
        {
            _examScheduleService = examScheduleService;
            _studentExamService = studentExamService;
            _classService = classService;
            _mapper = mapper;
        }

        [Route("lich-thi")]
        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetManageExamSchedulesPagingRequest()
            {
                UserId = HttpContext.Session.GetString("UserId"),
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _examScheduleService.GetAllMyExamSchedulesPaging(request);
            ViewBag.Keyword = keyword;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }

        [Route("giao-vien/lich-thi")]
        public async Task<IActionResult> AdminLichThi(int ClassID, string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetManageExamSchedulesPagingRequest()
            {
                ClassID = ClassID,
                UserId = HttpContext.Session.GetString("UserId"),
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _examScheduleService.GetAllMyExamAdminSchedulesPaging(request);
            ViewBag.Keyword = keyword;
            ViewBag.ClassID = ClassID;

            var list = await _classService.GetAllMyAdminClass(HttpContext.Session.GetString("UserId"));
            ViewBag.listCl = list;

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }

        [Route("giao-vien/ket-qua-thi")]
        public async Task<IActionResult> AdminKetQuaThi(int ExamScheduleID, string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetManageStudentExamPagingRequest()
            {
                ExamScheduleID = ExamScheduleID,
                UserId = HttpContext.Session.GetString("UserId"),
                Keyword = null,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _studentExamService.GetAllAdminPaging(request);
            ViewBag.Keyword = keyword;
            ViewBag.ExamScheduleID = ExamScheduleID;

            var list = await _examScheduleService.GetAllMyExamAdminSchedules(HttpContext.Session.GetString("UserId"));
            ViewBag.listES = list;

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }

        public async Task<IActionResult> Report(int ExamScheduleID, string keyword)
        {
            var request = new GetManageStudentExamPagingRequest()
            {
                ExamScheduleID = ExamScheduleID,
                Keyword = keyword,
                UserId = HttpContext.Session.GetString("UserId")
            };
            var data = await _studentExamService.GetAllAdmin(request);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Submissions");
                worksheet.Cell(1, 1).Value = "Tên lớp học: " + data.Items.ToList()[0].ClassName;
                worksheet.Cell(2, 1).Value = "Tên kỳ thi: " + data.Items.ToList()[0].ExamScheduleName;
                var currentRow = 3;
                worksheet.Cell(currentRow, 1).Value = "STT";
                worksheet.Cell(currentRow, 2).Value = "Họ";
                worksheet.Cell(currentRow, 3).Value = "Tên";
                worksheet.Cell(currentRow, 4).Value = "Thời gian thi";
                worksheet.Cell(currentRow, 5).Value = "Điểm";

                foreach (var submission in data.Items)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = currentRow - 3;
                    worksheet.Cell(currentRow, 2).Value = submission.FirstName;
                    worksheet.Cell(currentRow, 3).Value = submission.LastName;
                    worksheet.Cell(currentRow, 4).Value = submission.DateTimeStudentExam;
                    worksheet.Cell(currentRow, 5).Value = submission.Mark;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + " Bảng điểm kỳ thi" + data.Items.ToList()[0].ExamScheduleName;
                    return File(
                    content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        FileName + ".xlsx"
                    );
                }
            }
        }

        [HttpGet("them-ky-thi")]
        public IActionResult Create()
        {
            if (User.Identity != null)
            {
                var user = User.Identity.Name;
            }
            return View();
        }

        [HttpPost("them-ky-thi")]
        public async Task<IActionResult> Create([FromForm] ExamSchedulesCreateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);


            var result = await _examScheduleService.Create(request);
            if (result != null)
            {
                TempData["result"] = "Thêm mới kỳ thi thành công";
                return RedirectToAction("Details", "Class", new { id = request.ClassID });
            }

            ModelState.AddModelError("", "Thêm kỳ thi thất bại");
            return View(request);
        }

        [HttpPost("gui-mail-ky-thi")]
        public async Task<IActionResult> SendMail(int ClassID, int ExamScheduleID)
        {
            var students = await _classService.GetAllStudentByClassIDD(ClassID);
            foreach (var item in students)
            {
                Utilities.Helpers.SendMail.SendEmail(item.Email, "Thông báo từ Classroom", "<p>\n    <img class=\"image_resized\" style=\"width:24.92%;\" src=\"https://localhost:5000/img/logo.png\" alt=\"logo-white.svg\">\n</p>\n<p>\n    <span style=\"font-family:'Trebuchet MS', Helvetica, sans-serif;font-size:20px;\"><mark class=\"marker-yellow\"><strong>Thông báo từ Classroom</strong></mark></span>\n</p>\n<p>\n    <span style=\"font-family:'Trebuchet MS', Helvetica, sans-serif;\"><mark class=\"marker-yellow\">Lớp học của bạn vừa cập nhật một kỳ thi mới</mark></span>\n</p>\n<p>\n    <span style=\"font-family:'Trebuchet MS', Helvetica, sans-serif;\"><mark class=\"marker-yellow\">Nhấn vào &nbsp;</mark></span><a href=\"https://localhost:5000/\"><span style=\"font-family:'Trebuchet MS', Helvetica, sans-serif;\"><mark class=\"marker-yellow\">https://localhost:5000/</mark></span></a><span style=\"font-family:'Trebuchet MS', Helvetica, sans-serif;\"><mark class=\"marker-yellow\"> để kiểm tra.</mark></span>\n</p>\n", "");
            }
            TempData["result"] = "Đã gửi mail thành công";
            return RedirectToAction("OverView", "ExamSchedule", new { id = ExamScheduleID });
        }

        [HttpGet("ky-thi")]
        public async Task<IActionResult> OverView(int id)
        {
            var result = await _examScheduleService.GetById(id);
            if (result == null) NotFound();
            string UserId = HttpContext.Session.GetString("UserId");
            var studentExamsViewModel = await _studentExamService.GetById(id, UserId);
            if (studentExamsViewModel != null)
            {
                ViewBag.myMark = studentExamsViewModel.Mark;
            }
            else ViewBag.myMark = null;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(result);
        }

        [HttpGet("de-thi")]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _examScheduleService.GetById(id);
            if (result == null) NotFound();
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(result);
        }

        [HttpGet("chinh-sua-ky-thi")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _examScheduleService.GetById(id);
            if (result == null) return NotFound();
            var examSchedulesViewModel = _mapper.Map<ExamSchedulesViewModel, ExamSchedulesUpdateRequest>(result);
            return View(examSchedulesViewModel);
        }

        [HttpPost("chinh-sua-ky-thi")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] ExamSchedulesUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _examScheduleService.Update(request);
            TempData["result"] = "Cập nhật kỳ thi thành công";
            var homework = await _examScheduleService.GetById(request.ExamScheduleID);
            return RedirectToAction("Details", "ExamSchedule", new { id = homework.ExamScheduleID });
        }

        public async Task<IActionResult> Delete(int id, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View();
            var schedulesViewModel = await _examScheduleService.GetById(id);
            var result = await _examScheduleService.Delete(id);
            if (result != 0)
            {
                TempData["result"] = "Xoá kỳ thi thành công";
                return RedirectToAction("Details", "Class", new { id = schedulesViewModel.ClassID });
            }

            ModelState.AddModelError("", "Xoá kỳ thi thất bại");
            return Redirect(returnUrl);
        }

    }
}