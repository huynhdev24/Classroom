using AutoMapper;
using Classroom.Data;
using Classroom.Models.System.Users;

namespace Classroom.Models.Mappings;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<ApplicationUser, UserViewModel>();
        CreateMap<UserViewModel, ApplicationUser>();
    }
}