using AutoMapper;
using Classroom.Data;
using Classroom.Models.Catalog.ExamSchedules;

namespace Classroom.Models.Mappings;
public class ExamScheduleProfile : Profile
{
    public ExamScheduleProfile()
    {
        CreateMap<ExamSchedule, ExamSchedulesViewModel>();
        CreateMap<ExamSchedulesViewModel, ExamSchedulesUpdateRequest>();
    }
}