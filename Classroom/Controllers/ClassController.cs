using System.Text;
using Microsoft.AspNetCore.Mvc;
using Classroom.Models.Catalog.Classes;
using Classroom.Application.Catalog.Classes;
using AutoMapper;
using Classroom.Application.Catalog.Rooms;
using Classroom.Application.System.Users;
using ClosedXML.Excel;

namespace Classroom.Controllers
{
    public class ClassController : BaseController
    {
        private readonly IClassService _classService;
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classService"></param>
        /// <param name="mapper"></param>
        /// <param name="roomService"></param>
        /// <param name="userService"></param>
        /// <author>huynhdev24</author>
        public ClassController(
            IClassService classService,
            IMapper mapper,
            IRoomService roomService,
            IUserService userService)
        {
            _classService = classService;
            _mapper = mapper;
            _roomService = roomService;
            _userService = userService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [HttpGet("admin/quan-tri-lop-hoc")]
        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new ClassPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var classes = await _classService.GetAllClassPaging(request);
            ViewBag.Keyword = keyword;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(classes);
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
            var data = await _classService.GetAllClass();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Submissions");
                worksheet.Cell(1, 1).Value = "DANH SÁCH LỚP HỌC";
                var currentRow = 3;
                worksheet.Cell(currentRow, 1).Value = "STT";
                worksheet.Cell(currentRow, 2).Value = "Mã lớp học";
                worksheet.Cell(currentRow, 3).Value = "Tên lớp học";
                worksheet.Cell(currentRow, 4).Value = "Chủ đề";
                worksheet.Cell(currentRow, 5).Value = "Ngày tạo lớp học";
                worksheet.Cell(currentRow, 6).Value = "Trạng thái";
                worksheet.Cell(currentRow, 7).Value = "Công khai";
                worksheet.Cell(currentRow, 8).Value = "Học phí";

                foreach (var _class in data)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = currentRow - 3;
                    worksheet.Cell(currentRow, 2).Value = _class.ClassID;
                    worksheet.Cell(currentRow, 3).Value = _class.ClassName;
                    worksheet.Cell(currentRow, 4).Value = _class.Topic;
                    worksheet.Cell(currentRow, 5).Value = _class.DateCreated;
                    worksheet.Cell(currentRow, 6).Value = _class.Status;
                    worksheet.Cell(currentRow, 7).Value = _class.isPublic;
                    worksheet.Cell(currentRow, 8).Value = _class.Tuition;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + " Danh sách lớp học ";
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
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [Route("danh-sach-lop-hoc")]
        public async Task<IActionResult> MyClass(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new ClassPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var classes = await _classService.GetAllMyClassPaging(request, HttpContext.Session.GetString("UserId").ToString());

            ViewBag.Keyword = keyword;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(classes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [Route("giao-vien/danh-sach-lop-hoc")]
        public async Task<IActionResult> MyAdminClass(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new ClassPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var classes = await _classService.GetAllMyAdminClassPaging(request, HttpContext.Session.GetString("UserId").ToString());

            ViewBag.Keyword = keyword;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(classes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [HttpGet("tong-quan-lop-hoc")]
        public async Task<IActionResult> OverView(int id)
        {
            var result = await _classService.GetById(id);
            if (result == null) return Redirect("khong-tim-thay-lop-hoc");
            await _classService.AddViewCount(id);
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
        [HttpGet("chi-tiet-lop-hoc")]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _classService.GetById(id);
            if (result == null) return Redirect("khong-tim-thay-lop-hoc");
            await _classService.AddViewCount(id);
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [HttpGet("tao-lop-hoc")]
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
        [HttpPost("tao-lop-hoc")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ClassCreateRequest request)
        {
            request.UserName = User.Identity.Name;
            if (!ModelState.IsValid)
                return View(request);

            var result = await _classService.Create(request);

            await _roomService.Create(request.UserName, request.ClassName);

            if (result != 0)
            {
                TempData["result"] = "Thêm mới lớp học thành công";
                return RedirectToAction("OverView", "Class", new { id = result });
            }

            ModelState.AddModelError("", "Thêm lớp học thất bại");
            return View(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [Route("khong-tim-thay-lop-hoc")]
        public IActionResult NotFind()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [HttpPost, ActionName("JoinClass")]
        public async Task<IActionResult> JoinClass(int id)
        {
            var result = await _classService.JoinClass(id, User.Identity.Name);
            if (result.Contains("Tham gia lớp học thành công"))
            {
                TempData["result"] = result;
                return RedirectToAction("Details", new { id = id });
            }

            if (result.Contains("đã tham gia") == true)
            {
                return RedirectToAction("Details", new { id = id });
            }

            TempData["result"] = result;
            return RedirectToAction("OverView", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClassID"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [HttpPost("tham-gia-lop-hoc"), ActionName("JoinClassById")]
        public async Task<IActionResult> JoinClassById(string ClassID)
        {
            var @class = await _classService.GetById(ClassID.Trim());
            if (@class == null) return Redirect("khong-tim-thay-lop-hoc");
            var result = await _classService.JoinClass(@class.ID, User.Identity.Name);
            return RedirectToAction("OverView", new { id = @class.ID });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [HttpGet("chinh-sua-lop-hoc")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _classService.GetById(id);
            if (result == null) return Redirect("khong-tim-thay-lop-hoc");
            var classViewModel = _mapper.Map<ClassViewModel, ClassUpdateRequest>(result);
            return View(classViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [HttpPost("chinh-sua-lop-hoc")]
        [ValidateAntiForgeryToken]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Edit([FromForm] ClassUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            await _classService.Update(request);
            TempData["result"] = "Cập nhật lớp học thành công";
            return RedirectToAction("Details", "Class", new { id = request.ID });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [HttpGet("admin/chinh-sua-lop-hoc")]
        public async Task<IActionResult> AdminEdit(int id)
        {
            var result = await _classService.GetAdminById(id);
            if (result == null) return Redirect("/khong-tim-thay-lop-hoc");
            var classViewModel = _mapper.Map<ClassViewModel, ClassUpdateRequest>(result);
            return View(classViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [HttpPost("admin/chinh-sua-lop-hoc")]
        [ValidateAntiForgeryToken]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AdminEdit([FromForm] ClassUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            await _classService.Update(request);
            TempData["result"] = "Cập nhật lớp học thành công";
            return RedirectToAction("Index", "Class");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        public async Task<IActionResult> ChangeClassID(int id)
        {
            await _classService.ChangeClassID(id);
            TempData["result"] = "Đổi mã lớp thành công";
            return RedirectToAction("Details", "Class", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage()
        {
            string filePath = "";
            List<ClassImageUpdateRequest> list = new List<ClassImageUpdateRequest>();
            foreach (IFormFile image in Request.Form.Files)
            {
                ClassImageUpdateRequest classImageUpdateRequest = new ClassImageUpdateRequest();
                classImageUpdateRequest.ThumbnailImage = image;
                list.Add(classImageUpdateRequest);
            }

            foreach (ClassImageUpdateRequest classImageCreateRequest in list)
            {
                filePath = await _classService.UploadImage(classImageCreateRequest);
            }
            return Json(new { url = filePath });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _classService.GetById(id);
            if (result == null) return Redirect("khong-tim-thay-lop-hoc");
            return View(result);
        }

        // POST: Classes/Delete/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <author>huynhdev24</author>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _classService.Delete(id);
            if (result != 0)
            {
                TempData["result"] = "Xoá lớp học thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Xoá lớp học thất bại");
            var @class = await _classService.GetById(id);
            return View(@class);
        }
    }
}
