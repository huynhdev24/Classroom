using AutoMapper;
using Classroom.Application.Common.SignalR;
using Classroom.Data;
using Classroom.Models.Catalog.Submissions;

namespace Classroom.Models.Mappings
{
    /// <summary>
    /// SubmissionProfile
    /// </summary>
    public class SubmissionProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        /// <author>huynhdev24</author>
        public SubmissionProfile()
        {
            CreateMap<Submission, SubmissionViewModel>();
            CreateMap<SubmissionViewModel, SubmissionUpdateRequest>();
        }
    }
}
