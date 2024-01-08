using AutoMapper;
using Classroom.Data;
using Classroom.Models.Catalog.Notifications;

namespace Classroom.Models.Mappings;

/// <summary>
/// NotificationProfile
/// </summary>
public class NotificationProfile : Profile
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>huynhdev24</author>
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationViewModel>();
        CreateMap<NotificationViewModel, NotificationUpdateRequest>();
    }
}