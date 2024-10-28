using AutoMapper;
using PostService.Dtos;
using PostService.Models;
using UserService;

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

      CreateMap<GrpcUserModel, User>()
        .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.UserId))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.Posts, opt => opt.Ignore());
    }
  }
}