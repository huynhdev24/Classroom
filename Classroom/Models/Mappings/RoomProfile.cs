using AutoMapper;
using Classroom.Data;
using Classroom.Models.Catalog.Rooms;

namespace Classroom.Models.Mappings;

public class RoomProfile : Profile
{
    public RoomProfile()
    {
        CreateMap<Room, RoomViewModel>();
        CreateMap<RoomViewModel, Room>();
    }
}
