using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using Classroom.Data;
using Classroom.Models.Common;
using Classroom.Models.System.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Classroom.Application.System.Users
{
    /// <summary>
    /// UserService
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public UserService(UserManager<ApplicationUser> userManager
        , SignInManager<ApplicationUser> signInManager
        , IConfiguration configuration
        , IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> Register(RegisterRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                return "Tài khoản đã tồn tại";
            }
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return "Email đã tồn tại";
            }

            user = new ApplicationUser()
            {
                Dob = request.Dob,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                AccountBalance = 0
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                return "Đăng ký thành công";
            }
            return "Đăng ký không thất bại";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> Update(string id, ApplicationUser request)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id))
            {
                return "Email đã tồn tại";
            }
            var user = await _userManager.FindByIdAsync(id.ToString());
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.Dob = request.Dob;
            user.Email = request.Email;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return "Cập nhật thành công";
            }
            return "Cập nhật thất bại";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserViewModel> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return null;
            }
            var roles = await _userManager.GetRolesAsync(user);

            var userViewModel = _mapper.Map<ApplicationUser, UserViewModel>(user);
            userViewModel.Roles = roles;
            return userViewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public async Task<UserViewModel> GetByName(string UserName)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                return null;
            }
            var roles = await _userManager.GetRolesAsync(user);
            var userViewModel = _mapper.Map<ApplicationUser, UserViewModel>(user);
            userViewModel.Roles = roles;
            return userViewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public async Task<UserViewModel> GetByEmail(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return null;
            }
            var roles = await _userManager.GetRolesAsync(user);
            var userViewModel = _mapper.Map<ApplicationUser, UserViewModel>(user);
            userViewModel.Roles = roles;
            return userViewModel;
        }

        static Regex RemoveUnicode_rg = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static string RemoveUnicode(string strInput)
        {
            if (ReferenceEquals(RemoveUnicode_rg, null))
            {
                RemoveUnicode_rg = new Regex("p{IsCombiningDiacriticalMarks}+");
            }
            var temp = strInput.Normalize(NormalizationForm.FormD);
            return RemoveUnicode_rg.Replace(temp, string.Empty).Replace("đ", "d").Replace("Đ", "D").ToLower();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PagedResult<UserViewModel>> GetUsersPaging(GetUserPagingRequest request)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => RemoveUnicode(x.UserName).Contains(RemoveUnicode(request.Keyword))
                || RemoveUnicode(x.PhoneNumber).Contains(RemoveUnicode(request.Keyword))
                || RemoveUnicode(x.Email).Contains(RemoveUnicode(request.Keyword))
                || RemoveUnicode(x.FirstName).Contains(RemoveUnicode(request.Keyword))
                || RemoveUnicode(x.LastName).Contains(RemoveUnicode(request.Keyword)));
            }

            //3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize).OrderBy(x => x.FirstName).ToListAsync();

            var userViewModels = _mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(data);

            //4. Select and projection
            var pagedResult = new PagedResult<UserViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = userViewModels
            };
            return pagedResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public async Task<bool> DepositMoney(string UserName, decimal money)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            if (user is null) return false;
            user.AccountBalance += money;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }
    }
}