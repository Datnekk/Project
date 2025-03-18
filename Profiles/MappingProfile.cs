using AutoMapper;
using be.Dtos.Auth;
using be.Dtos.Booking;
using be.Dtos.Rooms;
using be.Dtos.Users;
using be.Models;

namespace be.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile(){
            // User Mappings
            CreateMap<User, UserReadDTO>(); // GET request mapping

            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name)); // POST request mapping
            
            CreateMap<UserUpdateDTO, User>(); // PUT request mapping

            // Room Mappings
            CreateMap<Room, RoomReadDTO>()
                .ForMember(dest => dest.RoomType, opt => opt.MapFrom(src => src.RoomType.ToString())); // GET request mapping

            CreateMap<RoomCreateDTO, Room>(); // POST request mapping

            CreateMap<RoomUpdateDTO, Room>(); // PUT request mapping

            // Booking Mappings
            CreateMap<Booking, BookingReadDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room)); // GET request mapping

            CreateMap<BookingCreateDTO, Booking>(); // POST request mapping

            CreateMap<BookingUpdateDTO, Booking>(); // PUT request mapping
        }
    }
}