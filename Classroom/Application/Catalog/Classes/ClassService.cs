using System.Security.Cryptography.X509Certificates;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Classroom.Utilities.Exceptions;
using System.Net.Http.Headers;
using Classroom.Application.Common;
using Classroom.Utilities.Constants;
using Microsoft.AspNetCore.Identity;
using Classroom.Data;
using Classroom.Models.Catalog.Classes;
using Classroom.Data.Enums;
using Classroom.Models.Common;
using Classroom.Models.Catalog.Comments;
using Classroom.Models.Catalog.Notifications;
using AutoMapper;
using Classroom.Models.Catalog.ExamSchedules;

namespace Classroom.Application.Catalog.Classes
{
    /// <summary>
    /// ClassService
    /// </summary>
    public class ClassService : IClassService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";

        public ClassService(ApplicationDbContext context
        , IStorageService storageService
        , UserManager<ApplicationUser> userManager
        , IMapper mapper)
        {
            _context = context;
            _storageService = storageService;
            _userManager = userManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<int> UpdateImage(int classID, ClassImageUpdateRequest request)
        {
            var _class = await _context.Classes.FindAsync(classID);
            if (_class == null) throw new ClassroomException($"Cannot find an class with id {classID}");

            if (_class.ImagePath != null)
            {
                _class.ImagePath = await this.SaveFile(request.ThumbnailImage);
            }
            _context.Classes.Update(_class);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public async Task AddViewCount(int ID)
        {
            var _class = await _context.Classes.FindAsync(ID);
            if (_class == null) throw new ClassroomException($"Cannot find a class {ID}");
            _class.ViewCount += 1;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public async Task<bool> ChangeClassID(int ID)
        {
            var _class = await _context.Classes.FindAsync(ID);
            if (_class == null) throw new ClassroomException($"Cannot find a class {ID}");
            string ClassID = SystemVariable.GetRanDomClassID(7);
            while(_context.Classes.FirstOrDefault(x=> x.ClassID == ClassID) != null){
                ClassID = SystemVariable.GetRanDomClassID(7);
            }
            _class.ClassID = ClassID;
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<int> Create(ClassCreateRequest request)
        {
            var _class = new Class()
            {
                ClassID = SystemVariable.GetRanDomClassID(7),
                ClassName = request.ClassName,
                Topic = request.Topic,
                ClassRoom = request.ClassRoom,
                Description = request.Description,
                Tuition = request.Tuition,
                DateCreated = DateTime.Now,
                ViewCount = 0,
                Status = Status.Active,
                isPublic = IsPublic.NotPublic,
            };

            // Save file
            if (request.ThumbnailImage != null)
            {
                _class.ImagePath = await this.SaveFile(request.ThumbnailImage);
            }
            _context.Classes.Add(_class);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByNameAsync(request.UserName);

            // Add teacher
            var classDetail = new ClassDetail()
            {
                ClassID = _class.ID,
                UserID = user.Id,
                IsTeacher = Teacher.Teacher
            };
            _context.ClassDetails.Add(classDetail);
            await _context.SaveChangesAsync();

            return _class.ID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public async Task<int> Delete(int ID)
        {
            var _class = await _context.Classes.FindAsync(ID);
            if (_class == null) throw new ClassroomException($"Cannot find a class {ID}");

            if (_class.ImagePath != null)
            {
                await _storageService.DeleteFileAsync(_class.ImagePath);
            }

            _context.Classes.Remove(_class);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PagedResult<ClassViewModel>> GetAllClassPaging(ClassPagingRequest request)
        {
            //1. Select
            var classes = _context.Classes
            .Include(x => x.ClassDetails)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize);

            //2. Filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                classes = classes.Where(x => x.ClassName.Contains(request.Keyword)
                    || x.ClassID.Contains(request.Keyword)
                    || x.Topic.Contains(request.Keyword));
            }

            var data = await classes.ToListAsync();

            var classViewModel = _mapper.Map<IEnumerable<Class>, IEnumerable<ClassViewModel>>(data);

            foreach (var item in classViewModel)
            {
                string idTeacher = _context.ClassDetails.FirstOrDefault(x => x.IsTeacher == Data.Enums.Teacher.Teacher && x.ClassID == item.ID).UserID;
                var userViewModel = await _userManager.FindByIdAsync(idTeacher);
                item.Teacher = userViewModel.FirstName + " " + userViewModel.LastName;
            }

            //3. Paging
            int totalRow = await classes.CountAsync();

            //4. Select and projection
            var pageResult = new PagedResult<ClassViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = classViewModel
            };
            return pageResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<ClassViewModel>> GetAllClass()
        {
            //1. Select
            var classes = _context.Classes.Include(x => x.ClassDetails);
            var data = await classes.ToListAsync();

            var classViewModel = _mapper.Map<IEnumerable<Class>, IEnumerable<ClassViewModel>>(data);

            foreach (var item in classViewModel)
            {
                string idTeacher = _context.ClassDetails.FirstOrDefault(x => x.IsTeacher == Data.Enums.Teacher.Teacher && x.ClassID == item.ID).UserID;
                var userViewModel = await _userManager.FindByIdAsync(idTeacher);
                item.Teacher = userViewModel.FirstName + " " + userViewModel.LastName;
            }
            return classViewModel.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<PagedResult<ClassViewModel>> GetAllMyClassPaging(ClassPagingRequest request, string UserId)
        {
            //1. Select
            var classes = _context.Classes
            .Include(x => x.ClassDetails)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize);



            //2. Filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                classes = classes.Where(x => x.ClassName.Contains(request.Keyword)
                    || x.ClassID.Contains(request.Keyword)
                    || x.Topic.Contains(request.Keyword));
            }

            var data = await classes.ToListAsync();

            foreach (var item in data.ToList())
            {
                bool check = false;
                foreach (var i in item.ClassDetails)
                {
                    if (i.UserID == UserId)
                    {
                        check = true;
                        break;
                    }
                }
                if (check == false)
                {
                    data.Remove(item);
                }
            }

            var classViewModel = _mapper.Map<IEnumerable<Class>, IEnumerable<ClassViewModel>>(data);

            foreach (var item in classViewModel)
            {
                string idTeacher = _context.ClassDetails.FirstOrDefault(x => x.IsTeacher == Data.Enums.Teacher.Teacher && x.ClassID == item.ID).UserID;
                var userViewModel = await _userManager.FindByIdAsync(idTeacher);
                item.Teacher = userViewModel.FirstName + " " + userViewModel.LastName;
            }

            //3. Paging
            int totalRow = await classes.CountAsync();

            //4. Select and projection
            var pageResult = new PagedResult<ClassViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = classViewModel
            };
            return pageResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<PagedResult<ClassViewModel>> GetAllMyAdminClassPaging(ClassPagingRequest request, string UserId)
        {
            //1. Select
            var classes = _context.Classes
            .Include(x => x.ClassDetails)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize);



            //2. Filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                classes = classes.Where(x => x.ClassName.Contains(request.Keyword)
                    || x.ClassID.Contains(request.Keyword)
                    || x.Topic.Contains(request.Keyword));
            }

            var data = await classes.ToListAsync();

            foreach (var item in data.ToList())
            {
                bool check = false;
                foreach (var i in item.ClassDetails)
                {
                    if (i.UserID == UserId && i.IsTeacher == Teacher.Teacher)
                    {
                        check = true;
                        break;
                    }
                }
                if (check == false)
                {
                    data.Remove(item);
                }
            }

            var classViewModel = _mapper.Map<IEnumerable<Class>, IEnumerable<ClassViewModel>>(data);

            foreach (var item in classViewModel)
            {
                string idTeacher = _context.ClassDetails.FirstOrDefault(x => x.IsTeacher == Data.Enums.Teacher.Teacher && x.ClassID == item.ID).UserID;
                var userViewModel = await _userManager.FindByIdAsync(idTeacher);
                item.Teacher = userViewModel.FirstName + " " + userViewModel.LastName;
            }

            //3. Paging
            int totalRow = await classes.CountAsync();

            //4. Select and projection
            var pageResult = new PagedResult<ClassViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = classViewModel
            };
            return pageResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<List<Class>> GetAllMyAdminClass(string? UserId)
        {
            //1. Select
            var classes = _context.Classes
            .Include(x => x.ClassDetails);

            var data = await classes.ToListAsync();

            foreach (var item in data.ToList())
            {
                bool check = false;
                foreach (var i in item.ClassDetails)
                {
                    if (i.UserID == UserId && i.IsTeacher == Teacher.Teacher)
                    {
                        check = true;
                        break;
                    }
                }
                if (check == false)
                {
                    data.Remove(item);
                }
            }
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PagedResult<ClassViewModel>> GetAllClassPagingHome(ClassPagingRequest request)
        {
            //1. Select
            var classes = _context.Classes.Where(x => x.isPublic == IsPublic.Public)
            .Include(x => x.ClassDetails)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize);

            //2. Filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                classes = classes.Where(x => x.ClassName.Contains(request.Keyword)
                    || x.ClassID.Contains(request.Keyword)
                    || x.Topic.Contains(request.Keyword));
            }

            var data = await classes.ToListAsync();

            var classViewModel = _mapper.Map<IEnumerable<Class>, IEnumerable<ClassViewModel>>(data);

            foreach (var item in classViewModel)
            {
                string idTeacher = _context.ClassDetails.FirstOrDefault(x => x.IsTeacher == Data.Enums.Teacher.Teacher && x.ClassID == item.ID).UserID;
                var userViewModel = await _userManager.FindByIdAsync(idTeacher);
                item.Teacher = userViewModel.FirstName + " " + userViewModel.LastName;
            }

            //3. Paging
            int totalRow = await classes.CountAsync();

            //4. Select and projection
            var pageResult = new PagedResult<ClassViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = classViewModel
            };
            return pageResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NotificationID"></param>
        /// <returns></returns>
        public async Task<ICollection<NotificationImage>> GetAllNotificationImageByNotificationID(int NotificationID)
        {
            //1. Select join
            var query = from ni in _context.NotificationImages
                        where ni.NotificationID == NotificationID
                        select new { ni };

            var data = await query.Select(x => new NotificationImage()
            {
                ImageID = x.ni.ImageID,
                ImagePath = x.ni.ImagePath,
                IsDefault = x.ni.IsDefault
            }).ToListAsync();

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NotificationID"></param>
        /// <returns></returns>
        public async Task<ICollection<CommentViewModel>> GetCommentsById(int NotificationID)
        {
            //1. Select join
            var query = from c in _context.Comments
                        join u in _userManager.Users on c.UserID equals u.Id into cu
                        from u in cu.DefaultIfEmpty()
                        select new { u, c };
            //2. filter
            if (NotificationID != null && NotificationID != 0)
            {
                query = query.Where(p => p.c.NotificationID == NotificationID);
            }

            var data = await query
                .Select(x => new CommentViewModel()
                {
                    CommentID = x.c.CommentID,
                    NotificationID = x.c.NotificationID,
                    UserID = x.c.UserID,
                    Avatar = x.u.Avatar,
                    FullName = x.u.FirstName + " " + x.u.LastName,
                    Content = x.c.Content,
                    DateTimeCreated = x.c.DateTimeCreated,
                    UserName = x.u.UserName
                }).ToListAsync();

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClassID"></param>
        /// <returns></returns>
        public async Task<ICollection<NotificationViewModel>> GetAllNotificationByClassID(int ClassID)
        {
            //1. Select join
            var query = from n in _context.Notifications
                        where n.ClassID == ClassID
                        select new { n };

            var data = await query.Select(x => new NotificationViewModel()
            {
                NotificationID = x.n.NotificationID,
                Title = x.n.Title,
                Content = x.n.Content,
                DateTimeCreated = x.n.DateTimeCreated,
                NotificationImages = _context.NotificationImages.Where(p => p.NotificationID == x.n.NotificationID).ToList()
            }).OrderByDescending(x => x.DateTimeCreated).ToListAsync();

            foreach (var item in data)
            {
                item.Comments = await GetCommentsById(item.NotificationID);
            }

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClassID"></param>
        /// <returns></returns>
        public async Task<ICollection<ExamSchedulesViewModel>> GetAllExamScheduleByClassID(int ClassID)
        {
            //1. Select join
            var query = from n in _context.ExamSchedules
                        where n.ClassID == ClassID
                        select new { n };

            var data = await query.Select(x => new ExamSchedulesViewModel()
            {
                ExamScheduleID = x.n.ExamScheduleID,
                ClassID = x.n.ClassID,
                ExamScheduleName = x.n.ExamScheduleName,
                DateTimeCreated = x.n.DateTimeCreated,
                ExamDateTime = x.n.ExamDateTime,
                ExamTime = x.n.ExamTime
            }).OrderByDescending(x => x.DateTimeCreated).ToListAsync();

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ClassViewModel> GetById(int id)
        {
            var @class = await _context.Classes.Include(x => x.ClassDetails).Include(x => x.Homeworks).Where(x => x.Status == Status.Active)
                .FirstOrDefaultAsync(x => x.ID == id);
            if (@class == null)
            {
                return null;
            }
            var classDetail = _context.ClassDetails.FirstOrDefault(x => x.ClassID == @class.ID && x.IsTeacher == Teacher.Teacher);
            if (classDetail == null) return null;
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == classDetail.UserID);


            foreach (var item in @class.ClassDetails)
            {
                var _user = await _userManager.FindByIdAsync(item.UserID);
                item.User = _user;
            }

            var classViewModel = _mapper.Map<Class, ClassViewModel>(@class);
            classViewModel.Teacher = user.FirstName + " " + user.LastName;
            classViewModel.StudentNumber = _context.ClassDetails.Where(c => c.ClassID == @class.ID).Count() - 1;
            classViewModel.TeacherImage = user.Avatar;
            classViewModel.TeacherUserName = user.UserName;
            classViewModel.Notifications = await GetAllNotificationByClassID(@class.ID);
            classViewModel.ExamSchedules = await GetAllExamScheduleByClassID(@class.ID);
            return classViewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ClassViewModel> GetAdminById(int id)
        {
            var @class = await _context.Classes.Include(x => x.ClassDetails).Include(x => x.Homeworks)
                .FirstOrDefaultAsync(x => x.ID == id);
            if (@class == null)
            {
                return null;
            }
            var classDetail = _context.ClassDetails.FirstOrDefault(x => x.ClassID == @class.ID && x.IsTeacher == Teacher.Teacher);
            if (classDetail == null) return null;
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == classDetail.UserID);


            foreach (var item in @class.ClassDetails)
            {
                var _user = await _userManager.FindByIdAsync(item.UserID);
                item.User = _user;
            }

            var classViewModel = _mapper.Map<Class, ClassViewModel>(@class);
            classViewModel.Teacher = user.FirstName + " " + user.LastName;
            classViewModel.StudentNumber = _context.ClassDetails.Where(c => c.ClassID == @class.ID).Count() - 1;
            classViewModel.TeacherImage = user.Avatar;
            classViewModel.TeacherUserName = user.UserName;
            classViewModel.Notifications = await GetAllNotificationByClassID(@class.ID);
            classViewModel.ExamSchedules = await GetAllExamScheduleByClassID(@class.ID);
            return classViewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClassID"></param>
        /// <returns></returns>
        public async Task<ClassViewModel> GetById(string ClassID)
        {
            var @class = await _context.Classes.Include(x => x.ClassDetails).Include(x => x.Homeworks)
                .FirstOrDefaultAsync(x => x.ClassID == ClassID);
            if (@class == null) return null;
            var classDetail = _context.ClassDetails.FirstOrDefault(x => x.ClassID == @class.ID && x.IsTeacher == Teacher.Teacher);
            if (classDetail == null) return null;
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == classDetail.UserID);
            if (@class == null)
            {
                return null;
            }

            foreach (var item in @class.ClassDetails)
            {
                var _user = await _userManager.FindByIdAsync(item.UserID);
                item.User = _user;
            }

            var classViewModel = _mapper.Map<Class, ClassViewModel>(@class);
            classViewModel.Teacher = user.FirstName + " " + user.LastName;
            classViewModel.StudentNumber = _context.ClassDetails.Where(c => c.ClassID == @class.ID).Count() - 1;
            classViewModel.TeacherImage = user.Avatar;
            classViewModel.TeacherUserName = user.UserName;
            classViewModel.Notifications = await GetAllNotificationByClassID(@class.ID);
            classViewModel.ExamSchedules = await GetAllExamScheduleByClassID(@class.ID);
            return classViewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<int> Update(ClassUpdateRequest request)
        {
            var _class = await _context.Classes.FindAsync(request.ID);
            if (_class == null) return 0;
            _class.ClassName = request.ClassName;
            _class.Topic = request.Topic;
            _class.ClassRoom = request.ClassRoom;
            _class.Description = request.Description;
            _class.isPublic = request.IsPublic;
            _class.Status = request.Status;

            if (_class.Status != request.Status)
                //Save image
                if (request.ThumbnailImage != null)
                {
                    if (_class.ImagePath != null)
                    {
                        _class.ImagePath = await this.SaveFile(request.ThumbnailImage);
                    }
                }
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="isPublic"></param>
        /// <returns></returns>
        public async Task<bool> UpdateIsPublic(int ID, IsPublic isPublic)
        {
            var _class = await _context.Classes.FindAsync(ID);
            if (_class == null) return false;
            _class.isPublic = isPublic;
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<bool> UpdateStatus(int ID, Status status)
        {
            var _class = await _context.Classes.FindAsync(ID);
            if (_class == null) return false;
            _class.Status = status;
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="tuition"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTuition(int ID, decimal tuition)
        {
            var _class = await _context.Classes.FindAsync(ID);
            if (_class == null) return false;
            _class.Tuition = tuition;
            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return "/" + USER_CONTENT_FOLDER_NAME + "/" + fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> UploadImage(ClassImageUpdateRequest request)
        {
            if (request != null)
            {
                return await this.SaveFile(request.ThumbnailImage);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public async Task<string> JoinClass(int ID, string UserName)
        {

            var _class = await _context.Classes.FirstOrDefaultAsync(x => x.ID == ID);
            if (_class == null) return $"Không tìm thấy lớp học {ID}";

            var user = await _userManager.FindByNameAsync(UserName);
            if (user == null) return $"Không tìm thấy người dùng {UserName}";

            var classDetailCheck = _context.ClassDetails.FirstOrDefault(x => x.ClassID == _class.ID && x.UserID == user.Id);
            if (classDetailCheck != null) return $"Học sinh {UserName} đã tham gia lớp học";

            if (_class.Tuition > user.AccountBalance) return "Số dư trong tài khoản không đủ!";

            // Add student
            var classDetail = new ClassDetail()
            {
                ClassID = _class.ID,
                UserID = user.Id,
                IsTeacher = Teacher.Student
            };
            _context.ClassDetails.Add(classDetail);
            user.AccountBalance = user.AccountBalance - _class.Tuition;
            await _context.SaveChangesAsync();
            if (_class.Tuition == 0)
                return "Tham gia lớp học thành công";
            return $"Tham gia lớp học thành công. Tài khoản -{_class.Tuition}đ";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClassID"></param>
        /// <returns></returns>
        public async Task<ICollection<ClassDetail>> GetAllStudentByClassID(int ClassID)
        {
            //1. Select join
            var query =
                        from cd in _context.ClassDetails
                        select new { cd };

            if (ClassID != null)
            {
                query = query.Where(x => x.cd.ClassID == ClassID);
            }

            //3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Select(x => new ClassDetail()
            {


                IsTeacher = x.cd.IsTeacher,

            }).ToListAsync();

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PagedResult<ClassDetailViewModel>> GetAllStudentByClassIDPaging(GetAllStudentInClassPagingRequest request)
        {
            //1. Select join
            var query = from st in _userManager.Users
                        join cd in _context.ClassDetails on st.Id equals cd.UserID into cdst
                        from cd in cdst.DefaultIfEmpty()
                        join c in _context.Classes on cd.ClassID equals c.ID
                        select new { cd, st, c };

            if (request.ClassID != null)
            {
                query = query.Where(x => x.cd.ClassID == request.ClassID);
            }

            //3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ClassDetailViewModel()
                {
                    Email = x.st.Email,
                    PhoneNumber = x.st.PhoneNumber,
                    UserName = x.st.UserName,
                    FirstName = x.st.FirstName,
                    UserID = x.st.Id,
                    Dob = x.st.Dob,
                    LastName = x.st.LastName,
                    IsTeacher = x.cd.IsTeacher,
                    ClassName = x.c.ClassName,
                    ClassID = x.c.ClassID,
                    Avatar = x.st.Avatar
                }).ToListAsync();

            //4. Select and projection
            var pagedResult = new PagedResult<ClassDetailViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };
            return pagedResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClassID"></param>
        /// <returns></returns>
        public async Task<List<ClassDetailViewModel>> GetAllStudentByClassIDD(int ClassID)
        {
            //1. Select join
            var query = from st in _userManager.Users
                        join cd in _context.ClassDetails on st.Id equals cd.UserID into cdst
                        from cd in cdst.DefaultIfEmpty()
                        join c in _context.Classes on cd.ClassID equals c.ID
                        select new { cd, st, c };

            query = query.Where(x => x.cd.ClassID == ClassID);

            var data = await query
                .Select(x => new ClassDetailViewModel()
                {
                    Email = x.st.Email,
                    PhoneNumber = x.st.PhoneNumber,
                    UserName = x.st.UserName,
                    FirstName = x.st.FirstName,
                    UserID = x.st.Id,
                    Dob = x.st.Dob,
                    LastName = x.st.LastName,
                    IsTeacher = x.cd.IsTeacher,
                    ClassName = x.c.ClassName,
                    ClassID = x.c.ClassID,
                    Avatar = x.st.Avatar
                }).ToListAsync();
                
            return data;
        }
    }
}

