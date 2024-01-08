using AutoMapper;
using Classroom.Data;
using Classroom.Models.Catalog.Homeworks;

namespace Classroom.Models.Mappings;

/// <summary>
/// HomeworkProfile
/// </summary>
public class HomeworkProfile : Profile
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>huynhdev24</author>
    public HomeworkProfile()
    {
        CreateMap<Homework, HomeworkViewModel>();
        CreateMap<HomeworkViewModel, HomeworkUpdateRequest>();
    }
}