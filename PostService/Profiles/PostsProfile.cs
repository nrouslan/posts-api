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
    }
  }
}