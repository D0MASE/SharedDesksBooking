using AutoMapper;
using SharedDesksBooking.Models;

namespace SharedDesksBooking.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Reservation, UserReservationDto>();
            CreateMap<Desk, DeskResponseDto>();

            CreateMap<CreateReservationRequest, Reservation>();
            CreateMap<Reservation, Reservation>().ForMember(desk => desk.Id, opt => opt.Ignore());

            CreateMap<Desk, DeskResponseDto>();
            CreateMap<Reservation, ReservationDto>();
        }
    }
}
