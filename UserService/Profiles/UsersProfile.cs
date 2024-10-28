using AutoMapper;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Profiles
{
  public class UsersProfile : Profile
  {
    public UsersProfile()
    {
      // Source -> Target
      CreateMap<User, ReadUserDto>();
      CreateMap<CreateUserDto, User>();
      CreateMap<ReadUserDto, PublishUserDto>();
      CreateMap<User, GrpcUserModel>()
        .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
    }
  }
}