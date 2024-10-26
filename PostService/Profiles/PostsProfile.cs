using AutoMapper;
using PostService.Dtos;
using PostService.Models;

namespace PostService.Profiles
{
  public class PostsProfile : Profile
  {
    public PostsProfile()
    {
      // Source -> Target
      CreateMap<User, ReadUserDto>();
      CreateMap<Post, ReadPostDto>();
      CreateMap<CreatePostDto, Post>();
      CreateMap<PublishUserDto, User>()
        .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
    }
  }
}