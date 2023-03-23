using Classroom.Data;
using Classroom.Core.Repositories;
using Classroom.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Classroom.Models.System.Users;
using Classroom.Application.System.Users;
using Classroom.Models.VNPAY;
using Classroom.Application.Common.VNPAY;
using Microsoft.AspNetCore.Authorization;

namespace Classroom.Controllers;

public class UserController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UserController(IUserService userService
    , SignInManager<ApplicationUser> signInManager
    , IUnitOfWork unitOfWork)
    {
        _userService = userService;
        _signInManager = signInManager;
        _unitOfWork = unitOfWork;
    }
    [Authorize(Policy = "RequireAdmin")]
    [Route("admin/quan-ly-nguoi-dung")]
    public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
    {
        var request = new GetUserPagingRequest()
        {
            Keyword = keyword,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        var data = await _userService.GetUsersPaging(request);
        ViewBag.Keyword = keyword;

        if (TempData["result"] != null)
        {
            ViewBag.SuccessMsg = TempData["result"];
        }

        return View(data);
    }
    
    public static Dictionary<string, string> vnp_TransactionStatus = new Dictionary<string, string>()
        {
            {"00","Giao dịch thành công" },
            {"01","Giao dịch chưa hoàn tất" },
            {"02","Giao dịch bị lỗi" },
            {"04","Giao dịch đảo (Khách hàng đã bị trừ tiền tại Ngân hàng nhưng GD chưa thành công ở VNPAY)" },
            {"05","VNPAY đang xử lý giao dịch này (GD hoàn tiền)" },
            {"06","VNPAY đã gửi yêu cầu hoàn tiền sang Ngân hàng (GD hoàn tiền)" },
            {"07","Giao dịch bị nghi ngờ gian lận" },
            {"09","GD Hoàn trả bị từ chối" }
        };

    [HttpGet("nap-tien")]
    public IActionResult Pay()
    {
        RequestPayment request = new RequestPayment();
        request.ThoiHanThanhToan = DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss");
        return View(request);
    }

    [HttpPost("nap-tien")]
    public IActionResult Pay([FromForm] RequestPayment request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        ViewBag.thongBaoLoi = "";
        if (string.IsNullOrEmpty(VNPayConfig.vnp_TmnCode) || string.IsNullOrEmpty(VNPayConfig.vnp_HashSecret))
        {
            ViewBag.thongBaoLoi = "Vui lòng cấu hình các tham số: vnp_TmnCode,vnp_HashSecret";
            return View(request);
        }
        OrderInfo order = new OrderInfo();
        order.OrderId = DateTime.Now.Ticks; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
        order.Amount = Int64.Parse(request.SoTien.Replace(",", "")); // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
        order.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending"
        order.CreatedDate = DateTime.Now;
        string locale = request.NgonNgu;
        VnPayLibrary vnpay = new VnPayLibrary();
        vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", VNPayConfig.vnp_TmnCode);
        vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString());//Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
        if (!string.IsNullOrEmpty(request.NganHang))
            vnpay.AddRequestData("vnp_BankCode", request.NganHang);
        vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString());
        if (!string.IsNullOrEmpty(locale))
            vnpay.AddRequestData("vnp_Locale", locale);
        else
            vnpay.AddRequestData("vnp_Locale", "vn");
        vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
        vnpay.AddRequestData("vnp_OrderType", request.LoaiHangHoa);//default value: other
        vnpay.AddRequestData("vnp_ReturnUrl", VNPayConfig.vnp_Returnurl);
        vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());// Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
                                                                     //Add Params of 2.1.0 Version
        vnpay.AddRequestData("vnp_ExpireDate", request.ThoiHanThanhToan);
        //Billing

        string paymentUrl = vnpay.CreateRequestUrl(VNPayConfig.vnp_Url, VNPayConfig.vnp_HashSecret);
        return Redirect(paymentUrl);
    }

    [Route("nap-tien-thanh-cong")]
    public async Task<IActionResult> VNPayReturn([FromQuery] VNPayReturn request)
    {
        if (request.vnp_TransactionStatus == "00")
        {
            await _userService.DepositMoney(User.Identity.Name, request.vnp_Amount / 100);
        }
        request.message = "Không xác định được trạng thái";
        if (vnp_TransactionStatus.ContainsKey(request.vnp_TransactionStatus))
            request.message = vnp_TransactionStatus[request.vnp_TransactionStatus];
        return View(request);
    }

    [HttpGet("admin/them-nguoi-dung")]
    [Authorize(Policy = "RequireAdmin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("admin/them-nguoi-dung")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> Create(RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return View();

        var result = await _userService.Register(request);
        if (result == "Đăng ký thành công")
        {
            TempData["result"] = "Thêm mới người dùng thành công";
            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", result);
        return View(request);
    }

    [HttpGet("admin/chi-tiet-nguoi-dung")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> Details(string id)
    {
        var result = await _userService.GetById(id);
        return View(result);
    }

    [HttpGet("admin/chinh-sua-nguoi-dung")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _unitOfWork.User.GetUser(id);
        var roles = _unitOfWork.Role.GetRoles();

        var userRoles = await _signInManager.UserManager.GetRolesAsync(user);

        var roleItems = roles.Select(role =>
            new SelectListItem(
                role.Name,
                role.Id,
                userRoles.Any(ur => ur.Contains(role.Name)))).ToList();

        var vm = new EditUserViewModel
        {
            User = user,
            Roles = roleItems
        };

        return View(vm);
    }

    [HttpPost("admin/chinh-sua-nguoi-dung")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> OnPostAsync(EditUserViewModel data)
    {
        var user = await _unitOfWork.User.GetUser(data.User.Id);
        if (user == null)
        {
            return NotFound();
        }

        var userRolesInDb = await _signInManager.UserManager.GetRolesAsync(user);

        var rolesToAdd = new List<string>();
        var rolesToDelete = new List<string>();

        foreach (var role in data.Roles)
        {
            var assignedInDb = userRolesInDb.FirstOrDefault(ur => ur == role.Text);
            if (role.Selected)
            {
                if (assignedInDb == null)
                {
                    rolesToAdd.Add(role.Text);
                }
            }
            else
            {
                if (assignedInDb != null)
                {
                    rolesToDelete.Add(role.Text);
                }
            }
        }

        if (rolesToAdd.Any())
        {
            await _signInManager.UserManager.AddToRolesAsync(user, rolesToAdd);
        }

        if (rolesToDelete.Any())
        {
            await _signInManager.UserManager.RemoveFromRolesAsync(user, rolesToDelete);
        }

        user.FirstName = data.User.FirstName;
        user.LastName = data.User.LastName;
        user.PhoneNumber = data.User.PhoneNumber;
        user.Dob = data.User.Dob;
        user.Email = data.User.Email;
        var result = await _userService.Update(user.Id, user);

        if (result == "Cập nhật thành công")
        {
            TempData["result"] = "Cập nhật thành công, vui lòng đăng nhập lại";
            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", result);
        return RedirectToAction("Edit", new { id = user.Id });
    }

    [HttpGet("admin/xoa-nguoi-dung")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userService.GetById(id);
        if (user != null)
        {
            var deleteRequest = new UserDeleteRequest()
            {
                Dob = user.Dob,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Id = id
            };
            return View(deleteRequest);
        }
        return RedirectToAction("Error", "Home");
    }

    [HttpPost("admin/xoa-nguoi-dung")]
    [Authorize(Policy = "RequireAdmin")]
    public async Task<IActionResult> Delete(UserDeleteRequest request)
    {
        if (!ModelState.IsValid)
            return View();

        var result = await _userService.Delete(request.Id);
        if (result == true)
        {
            TempData["result"] = "Xoá người dùng thành công";
            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", "Xoá người dùng thất bại");
        return View(request);
    }
}
