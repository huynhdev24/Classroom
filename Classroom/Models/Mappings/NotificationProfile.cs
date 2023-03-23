using AutoMapper;
using Classroom.Data;
using Classroom.Models.Catalog.Notifications;

namespace Classroom.Models.Mappings;
public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationViewModel>();
        CreateMap<NotificationViewModel, NotificationUpdateRequest>();
    }
}