using AutoMapper;
using Classroom.Data;
using Classroom.Models.Catalog.ExamSchedules;

namespace Classroom.Models.Mappings;

/// <summary>
/// ExamScheduleProfile
/// </summary>
public class ExamScheduleProfile : Profile
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>huynhdev24</author>
    public ExamScheduleProfile()
    {
        CreateMap<ExamSchedule, ExamSchedulesViewModel>();
        CreateMap<ExamSchedulesViewModel, ExamSchedulesUpdateRequest>();
    }
}