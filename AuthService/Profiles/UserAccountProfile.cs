using AuthService.Dtos;
using AuthService.Models;
using AutoMapper;

namespace AuthService.Profiles
{
  public class UserAccountProfile : Profile
  {
    public UserAccountProfile()
    {
      // Source -> Target
      CreateMap<SignUpRequestDto, UserAccount>();
      CreateMap<UserAccount, UserResponseDto>();
    }
  }
}