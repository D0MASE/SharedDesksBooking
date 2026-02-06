using AutoMapper;
using SharedDesksBooking.Models;

namespace SharedDesksBooking.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Reservation, UserReservationDto>();
        CreateMap<Desk, DeskResponseDto>();

        CreateMap<CreateReservationRequest, Reservation>();
        CreateMap<Reservation, Reservation>();

        CreateMap<Desk, DeskResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<Reservation, ReservationDto>();
    }
}
