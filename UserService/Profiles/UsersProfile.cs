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
      CreateMap<UpdateUserDto, User>();
      CreateMap<PublishedUserDto, User>();
      CreateMap<User, PublishUserDeleteDto>();
      CreateMap<User, PublishUserUpdateDto>();
    }
  }
}