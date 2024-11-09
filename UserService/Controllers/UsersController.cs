using System.Text.Json;
using AutoMapper;
using EventBusSDK;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Controllers
{
  [Route("api/users")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly IUserRepo _repository;

    private readonly IMapper _mapper;

    private readonly IPrincipalHelper _principalHelper;

    private readonly MessageBusPublisher _messageBusPublisher;

    public UsersController(
      IUserRepo repository,
      IMapper mapper,
      IPrincipalHelper principalHelper,
      MessageBusPublisher messageBusPublisher)
    {
      _repository = repository;
      _mapper = mapper;
      _principalHelper = principalHelper;
      _messageBusPublisher = messageBusPublisher;
    }

    /// <summary>
    /// Получение всех пользователей системы
    /// </summary>
    /// <returns>Коллекция ReadUserDto</returns>
    /// <response code="200">Пользователи системы</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<ReadUserDto>> GetAllUsers()
    {
      Console.WriteLine("--> Getting all users...");

      var users = _repository.GetAll();

      return Ok(_mapper.Map<IEnumerable<ReadUserDto>>(users));
    }

    /// <summary>
    /// Получение пользователя по Id
    /// </summary>
    /// <param name="id">Id пользователя</param>
    /// <returns>ReadUserDto</returns>
    /// <response code="200">Пользователь системы</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpGet("{id}", Name = nameof(GetUserById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ReadUserDto> GetUserById(int id)
    {
      Console.WriteLine($"--> Getting a user (userId: {id})...");

      var user = _repository.GetById(id);

      if (user != null)
      {
        return Ok(_mapper.Map<ReadUserDto>(user));
      }

      return NotFound();
    }

    /// <summary>
    /// Изменение данных пользователя
    /// </summary>
    /// <param name="id">Id пользователя</param>
    /// <param name="updateUserDto">Новые данные пользователя</param>
    /// <remarks>
    /// Пример запроса:
    ///
    ///     PUT api/users/{id}
    ///     {
    ///        "UserName": "annlad",
    ///        "Email": "annlad@gmail.com"
    ///     }
    ///     
    /// </remarks>
    /// <returns>ReadUserDto</returns>
    /// <response code="200">Новая информация о пользователе системы</response>
    /// <response code="401">Пользователь не авторизован</response>
    /// <response code="403">Запрет на изменение данных пользователя</response>
    /// <response code="404">Пользователь не найден</response>
    /// <response code="409">Данные уже заняты друми пользователями</response>
    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<ReadUserDto> UpdateUser(int id, UpdateUserDto updateUserDto)
    {
      Console.WriteLine($"--> Updating a user (userId: {id})...");

      var userInDb = _repository.GetById(id);

      if (userInDb == null)
      {
        return NotFound();
      }

      User? curUser = _principalHelper.ToUser(User);

      if (curUser == null)
      {
        return Unauthorized();
      }

      if (_repository.IsUserExists(updateUserDto.UserName, updateUserDto.Email))
      {
        return Conflict();
      }

      if (curUser.Email != userInDb.Email
        || curUser.UserName != userInDb.UserName)
      {
        return Forbid();
      }

      var user = _mapper.Map<User>(updateUserDto);

      user.Id = id;
      user.RegisteredAt = userInDb.RegisteredAt;

      _repository.Update(user);

      _repository.Save();

      // Asynchronously send a message of user deletion

      try
      {
        var publishUserUpdateDto = _mapper.Map<PublishUserUpdateDto>(userInDb);

        publishUserUpdateDto.Id = id;

        publishUserUpdateDto.Event = "UserUpdate";

        var message = JsonSerializer.Serialize(publishUserUpdateDto);

        _messageBusPublisher.Publish(message);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
      }

      return Ok(_mapper.Map<ReadUserDto>(user));
    }

    /// <summary>
    /// Удаление пользователя
    /// </summary>
    /// <param name="id">Id пользователя</param>
    /// <returns>ReadUserDto</returns>
    /// <response code="200">Удаленный пользователь системы</response>
    /// <response code="401">Пользователь не авторизован</response>
    /// <response code="403">Запрет на удаление пользователя</response>
    /// <response code="404">Пользователь не найден</response>
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    public ActionResult<ReadUserDto> DeleteUser(int id)
    {
      Console.WriteLine($"--> Deleting a user (userId: {id})...");

      var userInDb = _repository.GetById(id);

      if (userInDb == null)
      {
        return NotFound();
      }

      User? curUser = _principalHelper.ToUser(User);

      if (curUser == null)
      {
        return Unauthorized();
      }

      if (curUser.Email != userInDb.Email
        || curUser.UserName != userInDb.UserName)
      {
        return Forbid();
      }

      _repository.Delete(id);
      _repository.Save();

      // Asynchronously send a message of user deletion

      try
      {
        var publishUserDeleteDto = _mapper.Map<PublishUserDeleteDto>(userInDb);

        publishUserDeleteDto.Event = "UserDelete";

        var message = JsonSerializer.Serialize(publishUserDeleteDto);

        _messageBusPublisher.Publish(message);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
      }

      return Ok(_mapper.Map<ReadUserDto>(userInDb));
    }
  }
}