using Classroom.Data.Enums;
using Classroom.Data;
using Classroom.Models.Catalog.Classes;
using Classroom.Models.Common;

namespace Classroom.Application.Catalog.Classes
{
    public interface IClassService
    {
        Task<int> Create(ClassCreateRequest request);
        Task<string> UploadImage(ClassImageUpdateRequest request);
        Task<int> Update(ClassUpdateRequest request);
        Task<int> Delete(int ID);
        Task<ClassViewModel> GetById(int ID);
        Task<ClassViewModel> GetAdminById(int ID);
        Task<ClassViewModel> GetById(string ClassID);
        Task<bool> UpdateTuition(int ID, decimal tuition);
        Task<bool> UpdateStatus(int ID, Status status);
        Task<bool> UpdateIsPublic(int ID, IsPublic isPublic);
        Task AddViewCount(int ID);
        Task<PagedResult<ClassViewModel>> GetAllClassPaging(ClassPagingRequest request);
        Task<List<ClassViewModel>> GetAllClass();
        Task<PagedResult<ClassViewModel>> GetAllMyClassPaging(ClassPagingRequest request, string UserId);
        Task<PagedResult<ClassViewModel>> GetAllMyAdminClassPaging(ClassPagingRequest request, string UserId);
        Task<List<Class>> GetAllMyAdminClass(string? UserId);
        Task<PagedResult<ClassViewModel>> GetAllClassPagingHome(ClassPagingRequest request);
        Task<PagedResult<ClassDetailViewModel>> GetAllStudentByClassIDPaging(GetAllStudentInClassPagingRequest request);
        Task<List<ClassDetailViewModel>> GetAllStudentByClassIDD(int ClassID);
        Task<int> UpdateImage(int classID, ClassImageUpdateRequest request);
        Task<bool> ChangeClassID(int ID);
        Task<string> JoinClass(int ClassID, string UserName);
    }
}