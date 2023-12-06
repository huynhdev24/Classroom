using AutoMapper;
using Classroom.Data;
using Classroom.Models.Catalog.Question;

namespace Classroom.Models.Mappings;

/// <summary>
/// QuestionProfile
/// </summary>
public class QuestionProfile : Profile
{
    public QuestionProfile()
    {
        CreateMap<Question, QuestionViewModel>();
        CreateMap<QuestionsCreateRequest, Question>();
        CreateMap<QuestionViewModel, QuestionUpdateRequest>();
    }
}