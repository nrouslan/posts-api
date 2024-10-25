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
    }
  }
}