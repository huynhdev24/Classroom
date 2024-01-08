using AutoMapper;
using Classroom.Data;
using Classroom.Models.System.Users;

namespace Classroom.Models.Mappings;

/// <summary>
/// UserProfile
/// </summary>
public class UserProfile : Profile
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>huynhdev24</author>
    public UserProfile()
    {
        CreateMap<ApplicationUser, UserViewModel>();
        CreateMap<UserViewModel, ApplicationUser>();
    }
}