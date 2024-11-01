using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PostService.Data;
using PostService.Dtos;
using PostService.Models;

namespace PostService.Controllers
{
  [Route("api/users/{userId}/posts")]
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
      Console.WriteLine($"--> Getting posts for user (userId: {userId})...");

      var posts = _repository.GetPostsForUser(userId);

      return Ok(_mapper.Map<IEnumerable<ReadPostDto>>(posts));
    }

    [HttpGet("{postId}", Name = "GetPostById")]
    public ActionResult<ReadPostDto> GetPostById(int userId, int postId)
    {
      Console.WriteLine($"--> Getting a post (userId: {userId}, postId: {postId})...");

      var post = _repository.GetPostById(userId, postId);

      if (post == null)
      {
        return NotFound();
      }

      return Ok(_mapper.Map<ReadPostDto>(post));
    }

    [HttpPost]
    public ActionResult<ReadPostDto> CreatePost(int userId, CreatePostDto createPostDto)
    {
      Console.WriteLine($"--> Creating a post (userId: {userId})...");

      var post = _mapper.Map<Post>(createPostDto);

      post.UserId = userId;

      _repository.Insert(post);

      _repository.SaveChanges();

      var readPostDto = _mapper.Map<ReadPostDto>(post);

      return CreatedAtRoute(nameof(GetPostById),
        new { userId = post.UserId, postId = readPostDto.Id }, readPostDto);
    }

    [HttpPut("{postId}")]
    public ActionResult<ReadPostDto> UpdatePost(int userId, int postId, UpdatePostDto updatePostDto)
    {
      Console.WriteLine($"--> Updating a post (userId: {userId}, postId: {postId})...");

      var postInDb = _repository.GetPostById(userId, postId);

      if (postInDb == null)
      {
        return NotFound();
      }

      var post = _mapper.Map<Post>(updatePostDto);

      post.Id = postId;

      _repository.Update(userId, post);

      _repository.SaveChanges();

      return Ok(_mapper.Map<ReadPostDto>(post));
    }

    [HttpDelete("{postId}")]
    public ActionResult<ReadPostDto> DeletePost(int userId, int postId)
    {
      Console.WriteLine($"--> Deleting a post (userId: {userId}, postId: {postId})...");

      var post = _repository.GetPostById(userId, postId);

      if (post == null)
      {
        return NotFound();
      }

      _repository.Delete(userId, postId);

      _repository.SaveChanges();

      return Ok(_mapper.Map<ReadPostDto>(post));
    }
  }
}