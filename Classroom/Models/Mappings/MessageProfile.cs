using AutoMapper;
using Classroom.Application.Common.SignalR;
using Classroom.Data;
using Classroom.Models.Catalog.Messages;

namespace Classroom.Models.Mappings
{
    /// <summary>
    /// MessageProfile
    /// </summary>
    public class MessageProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        /// <author>huynhdev24</author>
        public MessageProfile()
        {
            CreateMap<Message, MessageViewModel>()
                .ForMember(dst => dst.From, opt => opt.MapFrom(x => x.FromUser != null ? x.FromUser.FirstName + " " + x.FromUser.LastName : null))
                .ForMember(dst => dst.Room, opt => opt.MapFrom(x => x.ToRoom != null ? x.ToRoom.Name : null))
                .ForMember(dst => dst.Avatar, opt => opt.MapFrom(x => x.FromUser != null ? x.FromUser.Avatar : null))
                .ForMember(dst => dst.Content, opt => opt.MapFrom(x => BasicEmojis.ParseEmojis(x.Content != null ? x.Content : "" )));

            CreateMap<MessageViewModel, Message>();
        }
    }
}
