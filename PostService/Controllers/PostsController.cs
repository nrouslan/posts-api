using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

    private readonly IUsersDataClient _usersDataClient;

    private readonly IPrincipalHelper _principalHelper;

    public PostsController(
      IPostRepo repository,
      IMapper mapper,
      IUsersDataClient usersDataClient,
      IPrincipalHelper principalHelper)
    {
      _repository = repository;
      _mapper = mapper;
      _usersDataClient = usersDataClient;
      _principalHelper = principalHelper;
    }

    // FIXME: Add check if the user exists 

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

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ReadPostDto>> CreatePost(int userId, CreatePostDto createPostDto)
    {
      Console.WriteLine($"--> Creating a post (userId: {userId})...");

      // Authorize the user
      try
      {
        var userInDb = await _usersDataClient.GetUserById(userId);

        if (userInDb == null)
        {
          return NotFound();
        }

        var curUser = await _principalHelper.ToUser(User);

        if (curUser == null
          || curUser.Email != userInDb.Email
          || curUser.UserName != userInDb.UserName)
        {
          return Unauthorized();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not get synchronously a user: {ex.Message}");

        return StatusCode(500, "Произошла внутренняя ошибка сервера.");
      }

      var post = _mapper.Map<Post>(createPostDto);

      post.UserId = userId;

      _repository.Insert(post);

      _repository.SaveChanges();

      var readPostDto = _mapper.Map<ReadPostDto>(post);

      return CreatedAtRoute(nameof(GetPostById),
        new { userId = post.UserId, postId = readPostDto.Id }, readPostDto);
    }

    [Authorize]
    [HttpPut("{postId}")]
    public async Task<ActionResult<ReadPostDto>> UpdatePost(
      int userId,
      int postId,
      UpdatePostDto updatePostDto)
    {
      Console.WriteLine($"--> Updating a post (userId: {userId}, postId: {postId})...");

      // Authorize the user
      try
      {
        var userInDb = await _usersDataClient.GetUserById(userId);

        if (userInDb == null)
        {
          return NotFound();
        }

        var curUser = await _principalHelper.ToUser(User);

        if (curUser == null
          || curUser.Email != userInDb.Email
          || curUser.UserName != userInDb.UserName)
        {
          return Unauthorized();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not get synchronously a user: {ex.Message}");

        return StatusCode(500, "Произошла внутренняя ошибка сервера.");
      }

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

    [Authorize]
    [HttpDelete("{postId}")]
    public async Task<ActionResult<ReadPostDto>> DeletePost(int userId, int postId)
    {
      Console.WriteLine($"--> Deleting a post (userId: {userId}, postId: {postId})...");

      // Authorize the user
      try
      {
        var userInDb = await _usersDataClient.GetUserById(userId);

        if (userInDb == null)
        {
          return NotFound();
        }

        var curUser = await _principalHelper.ToUser(User);

        if (curUser == null
          || curUser.Email != userInDb.Email
          || curUser.UserName != userInDb.UserName)
        {
          return Unauthorized();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not get synchronously a user: {ex.Message}");

        return StatusCode(500, "Произошла внутренняя ошибка сервера.");
      }

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