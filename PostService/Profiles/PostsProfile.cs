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

      CreateMap<Post, ReadPostDto>();

      CreateMap<CreatePostDto, Post>();

      CreateMap<UpdatePostDto, Post>();

      CreateMap<ReadUserDto, User>();

      CreateMap<PublishUserUpdateDto, User>();

      CreateMap<PublishedUserDto, User>();
    }
  }
}