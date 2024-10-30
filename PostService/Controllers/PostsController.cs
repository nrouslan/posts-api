using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PostService.Data;
using PostService.Dtos;
using PostService.Models;

namespace PostService.Controllers
{
  [Route("api/users/{userId}/[controller]")]
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
    public ActionResult<IEnumerable<ReadPostDto>> GetPostsForUser(Guid userId)
    {
      Console.WriteLine($"--> Getting posts for user with id: {userId}...");

      var posts = _repository.GetPostsForUser(userId);

      return Ok(_mapper.Map<IEnumerable<ReadPostDto>>(posts));
    }

    [HttpGet("{postId::guid}", Name = "GetPostById")]
    public ActionResult<ReadPostDto> GetPostById(Guid postId)
    {
      Console.WriteLine($"--> Getting post with id: {postId}...");

      var post = _repository.GetPostById(postId);

      if (post == null)
      {
        return NotFound();
      }

      return Ok(_mapper.Map<ReadPostDto>(post));
    }

    [HttpPost]
    public ActionResult<ReadPostDto> CreatePost(Guid userId, CreatePostDto createPostDto)
    {
      Console.WriteLine($"--> Creating a post for user with id: {userId}...");

      var post = _mapper.Map<Post>(createPostDto);

      post.UserId = userId;

      _repository.Insert(post);

      _repository.SaveChanges();

      var readPostDto = _mapper.Map<ReadPostDto>(post);

      return CreatedAtRoute(nameof(GetPostById),
        new { post.UserId, postId = readPostDto.Id }, readPostDto);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<ReadPostDto> UpdatePost(Guid userId, Guid id, UpdatePostDto updatePostDto)
    {
      Console.WriteLine($"--> Updating a post with id: {id}...");

      var postInDb = _repository.GetPostById(id);

      if (postInDb == null)
      {
        return NotFound();
      }

      var post = _mapper.Map<Post>(updatePostDto);

      post.UserId = userId;
      post.Id = id;

      _repository.Update(post);

      _repository.SaveChanges();

      return Ok(_mapper.Map<ReadPostDto>(post));
    }

    [HttpDelete("{id:guid}")]
    public ActionResult<ReadPostDto> DeletePost(Guid id)
    {
      Console.WriteLine($"--> Deleting a post with id: {id}...");

      var post = _repository.GetPostById(id);

      if (post == null)
      {
        return NotFound();
      }

      _repository.Delete(id);

      _repository.SaveChanges();

      return Ok(_mapper.Map<ReadPostDto>(post));
    }
  }
}