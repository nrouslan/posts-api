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

    /// <summary>
    /// Получение всех постов пользователя системы
    /// </summary>
    /// <returns>Коллекция ReadPostDto</returns>
    /// <response code="200">Посты пользователя системы</response>
    /// <response code="503">Запрос не может быть обработан из-за зависимости от неработающего сервиса</response>
    /// <response code="404">Пользователь не найден</response>

    [HttpGet]

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ReadPostDto>>> GetPostsForUser(int userId)
    {
      Console.WriteLine($"--> Getting posts for user (userId: {userId})...");

      try
      {
        var user = await _usersDataClient.GetUserById(userId);

        if (user == null)
        {
          return NotFound();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not get synchronously a user: {ex.Message}");

        return StatusCode(503,
          "Запрос не может быть обработан из-за зависимости от неработающего сервиса.");
      }

      var posts = _repository.GetPostsForUser(userId);

      return Ok(_mapper.Map<IEnumerable<ReadPostDto>>(posts));
    }

    /// <summary>
    /// Получение поста пользователя по Id
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="postId">Id поста</param>
    /// <returns>ReadPostDto</returns>
    /// <response code="200">Пост пользователя</response>
    /// <response code="503">Запрос не может быть обработан из-за зависимости от неработающего сервиса</response>
    /// <response code="404">Пост или пользователь не найден</response>

    [HttpGet("{postId}", Name = "GetPostById")]

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReadPostDto>> GetPostById(int userId, int postId)
    {
      Console.WriteLine($"--> Getting a post (userId: {userId}, postId: {postId})...");

      try
      {
        var userInDb = await _usersDataClient.GetUserById(userId);

        var post = _repository.GetPostById(userId, postId);

        if (userInDb == null || post == null)
        {
          return NotFound();
        }

        return Ok(_mapper.Map<ReadPostDto>(post));
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not get synchronously a user: {ex.Message}");

        return StatusCode(503,
          "Запрос не может быть обработан из-за зависимости от неработающего сервиса.");
      }
    }

    /// <summary>
    /// Создание поста для пользователя
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="createPostDto">Данные нового поста</param>
    /// <remarks>
    /// Пример запроса:
    ///
    ///     POST api/users/{userId}/posts
    ///     {
    ///        "Title": "Post Title",
    ///        "Content": "Post Content",
    ///        "PublishedAt": "2024-11-01T23:00:00Z",
    ///     }
    ///     
    /// </remarks>
    /// <returns>ReadPostDto</returns>
    /// <response code="200">Новый пост пользователя</response>
    /// <response code="503">Запрос не может быть обработан из-за зависимости от неработающего сервиса</response>
    /// <response code="401">Пользователь не авторизован</response>
    /// <response code="403">Запрет на создание постов для пользователя</response>
    /// <response code="404">Пользователь не найден</response>

    [Authorize]
    [HttpPost]

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ReadPostDto>> CreatePost(int userId, CreatePostDto createPostDto)
    {
      Console.WriteLine($"--> Creating a post (userId: {userId})...");

      try
      {
        var userInDb = await _usersDataClient.GetUserById(userId);

        var curUser = await _principalHelper.ToUser(User);

        if (userInDb == null || curUser == null)
        {
          return NotFound();
        }

        if (curUser.Email != userInDb.Email
          || curUser.UserName != userInDb.UserName)
        {
          return Unauthorized();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not get synchronously a user: {ex.Message}");

        return StatusCode(503,
          "Запрос не может быть обработан из-за зависимости от неработающего сервиса.");
      }

      var post = _mapper.Map<Post>(createPostDto);

      post.UserId = userId;

      _repository.Insert(post);

      _repository.SaveChanges();

      var readPostDto = _mapper.Map<ReadPostDto>(post);

      return CreatedAtRoute(nameof(GetPostById),
        new { userId = post.UserId, postId = readPostDto.Id }, readPostDto);
    }

    /// <summary>
    /// Изменение поста для пользователя
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="postId">Id поста</param>
    /// <param name="updatePostDto">Данные данные для поста</param>
    /// <remarks>
    /// Пример запроса:
    ///
    ///     PUT api/users/{userId}/posts/{postId}
    ///     {
    ///        "Title": "New Post Title",
    ///        "Content": "New Post Content",
    ///     }
    ///     
    /// </remarks>
    /// <returns>ReadPostDto</returns>
    /// <response code="200">Новый пост пользователя</response>
    /// <response code="503">Запрос не может быть обработан из-за зависимости от неработающего сервиса</response>
    /// <response code="401">Пользователь не авторизован</response>
    /// <response code="403">Запрет на создание постов для пользователя</response>
    /// <response code="404">Пост или пользователь не найден</response>

    [Authorize]
    [HttpPut("{postId}")]

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ReadPostDto>> UpdatePost(
      int userId,
      int postId,
      UpdatePostDto updatePostDto)
    {
      Console.WriteLine($"--> Updating a post (userId: {userId}, postId: {postId})...");

      try
      {
        var userInDb = await _usersDataClient.GetUserById(userId);

        var curUser = await _principalHelper.ToUser(User);

        if (userInDb == null || curUser == null)
        {
          return NotFound();
        }

        if (curUser.Email != userInDb.Email
          || curUser.UserName != userInDb.UserName)
        {
          return Unauthorized();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not get synchronously a user: {ex.Message}");

        return StatusCode(503,
          "Запрос не может быть обработан из-за зависимости от неработающего сервиса.");
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

    /// <summary>
    /// Удаление поста пользователя
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="postId">Id поста</param>
    /// <returns>ReadPostDto</returns>
    /// <response code="200">Удаленный пост пользователя</response>
    /// <response code="503">Запрос не может быть обработан из-за зависимости от неработающего сервиса</response>
    /// <response code="401">Пользователь не авторизован</response>
    /// <response code="403">Запрет на удаление постов для пользователя</response>
    /// <response code="404">Пост или пользователь не найден</response>

    [Authorize]
    [HttpDelete("{postId}")]

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ReadPostDto>> DeletePost(int userId, int postId)
    {
      Console.WriteLine($"--> Deleting a post (userId: {userId}, postId: {postId})...");

      try
      {
        var userInDb = await _usersDataClient.GetUserById(userId);

        var curUser = await _principalHelper.ToUser(User);

        if (userInDb == null || curUser == null)
        {
          return NotFound();
        }

        if (curUser.Email != userInDb.Email
          || curUser.UserName != userInDb.UserName)
        {
          return Unauthorized();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not get synchronously a user: {ex.Message}");

        return StatusCode(503,
          "Запрос не может быть обработан из-за зависимости от неработающего сервиса.");
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