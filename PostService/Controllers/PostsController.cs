using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PostService.Data;
using PostService.Dtos;
using PostService.Models;

namespace PostService.Controllers
{
  [Route("p/api/users/{userId}/[controller]")]
  [ApiController]
  public class PostsController : ControllerBase
  {
    private readonly IPostRepo _repository;
    private readonly IMapper _mapper;

    public PostsController(IPostRepo repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ReadPostDto>> GetPostsForUser(int userId)
    {
      Console.WriteLine($"--> Hit {nameof(GetPostsForUser)}: {userId}");

      if (!_repository.IsUserExists(userId))
      {
        return NotFound();
      }

      var posts = _repository.GetPostsForUser(userId);

      return Ok(_mapper.Map<IEnumerable<ReadPostDto>>(posts));
    }

    [HttpGet("{postId}", Name = "GetPostForUser")]
    public ActionResult<ReadPostDto> GetPostForUser(int userId, int postId)
    {
      Console.WriteLine($"--> Hit {nameof(GetPostForUser)}: {userId} / {postId}");

      if (!_repository.IsUserExists(userId))
      {
        return NotFound();
      }

      var post = _repository.GetPost(userId, postId);

      if (post == null)
      {
        return NotFound();
      }

      return Ok(_mapper.Map<ReadPostDto>(post));
    }

    [HttpPost]
    public ActionResult<ReadPostDto> CreatePostForUser(int userId, CreatePostDto createPostDto)
    {
      Console.WriteLine($"--> Hit {nameof(CreatePostForUser)}: {userId}");

      if (!_repository.IsUserExists(userId))
      {
        return NotFound();
      }

      var post = _mapper.Map<Post>(createPostDto);

      _repository.CreatePost(userId, post);

      _repository.SaveChanges();

      var readPostDto = _mapper.Map<ReadPostDto>(post);

      return CreatedAtRoute(nameof(GetPostForUser),
        new { userId, postId = readPostDto.Id }, readPostDto);
    }
  }
}