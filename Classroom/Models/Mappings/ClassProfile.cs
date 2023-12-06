using AutoMapper;
using Classroom.Data;
using Classroom.Models.Catalog.Classes;

namespace Classroom.Models.Mappings;

/// <summary>
/// ClassProfile
/// </summary>
public class ClassProfile : Profile
{
    public ClassProfile()
    {
        CreateMap<Class, ClassViewModel>()
            .ForMember(dst => dst.Image, opt => opt.MapFrom(x => x.ImagePath));
        CreateMap<ClassViewModel, ClassUpdateRequest>();
    }
}