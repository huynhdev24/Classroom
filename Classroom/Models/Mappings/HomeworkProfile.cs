using AutoMapper;
using Classroom.Data;
using Classroom.Models.Catalog.Homeworks;

namespace Classroom.Models.Mappings;
public class HomeworkProfile : Profile
{
    public HomeworkProfile()
    {
        CreateMap<Homework, HomeworkViewModel>();
        CreateMap<HomeworkViewModel, HomeworkUpdateRequest>();
    }
}