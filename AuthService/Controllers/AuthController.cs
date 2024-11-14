using System.Text.Json;
using AuthService.Auth;
using AuthService.Data;
using AuthService.Dtos;
using AuthService.Models;
using AutoMapper;
using EventBusSDK;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly IUserAccountRepo _userAccountRepo;

    private readonly IMapper _mapper;

    private readonly MessageBusPublisher _messageBusPublisher;

    public AuthController(
      IUserAccountRepo userAccountRepo,
      IMapper mapper,
      MessageBusPublisher messageBusPublisher)
    {
      _userAccountRepo = userAccountRepo;
      _mapper = mapper;
      _messageBusPublisher = messageBusPublisher;
    }

    /// <summary>
    /// Логин пользователя
    /// </summary>
    /// <remarks>
    /// Пример запроса:
    ///
    ///     POST api/auth/signin
    ///     {
    ///        "Email": "annlad@gmail.com",
    ///        "Password": "12345678"
    ///     }
    ///     
    /// </remarks>
    /// <returns>AuthResponseDto</returns>
    /// <response code="200">Пользователь и состояние входа</response>
    [HttpPost("signin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<AuthResponseDto> SignUserIn(
      [FromBody] SignInRequestDto signInRequestDto)
    {
      Console.WriteLine("--> Signing in a user...");

      var userAccount = _userAccountRepo.GetUserAccountByEmail(signInRequestDto.Email);

      if (userAccount == null
        || userAccount.Email != signInRequestDto.Email
        || !PasswordHasher.VerifyPassword(
          signInRequestDto.Password,
          userAccount.PasswordHash,
          Convert.FromHexString(userAccount.PasswordSalt)
        ))
      {
        return Ok(
          new AuthResponseDto()
          {
            User = null,
            IsSuccessful = false,
            JwtToken = null,
            Message = "Неверная почта и/или пароль!"
          }
        );
      }

      var userResponseDto = _mapper.Map<UserResponseDto>(userAccount);

      return Ok(
        new AuthResponseDto
        {
          User = userResponseDto,
          IsSuccessful = true,
          JwtToken = JwtAuthHandler.GenerateToken(userAccount),
          Message = null
        }
      );
    }

    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <remarks>
    /// Пример запроса:
    ///
    ///     POST api/auth/signup
    ///     {
    ///        "UserName": annlad
    ///        "Email": "annlad@gmail.com",
    ///        "Password": "12345678"
    ///     }
    ///     
    /// </remarks>
    /// <returns>AuthResponseDto</returns>
    /// <response code="200">Пользователь и состояние регистрации</response>
    [HttpPost("signup")]
    public ActionResult<AuthResponseDto> SignUserUp(
      [FromBody] SignUpRequestDto signUpRequestDto
    )
    {
      Console.WriteLine("--> Signing up a user...");

      if (_userAccountRepo.IsUserAccountExist(
        signUpRequestDto.UserName, signUpRequestDto.Email))
      {
        return Ok(
          new AuthResponseDto()
          {
            User = null,
            IsSuccessful = false,
            JwtToken = null,
            Message = "Почта и/или имя пользователя уже заняты!"
          }
        );
      }

      var userAccount = _mapper.Map<UserAccount>(signUpRequestDto);

      userAccount.RegisteredAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");

      // Hash incoming password
      userAccount.PasswordHash = PasswordHasher.HashPassword(signUpRequestDto.Password, out var salt);
      userAccount.PasswordSalt = Convert.ToHexString(salt);

      Console.WriteLine(
        $"--> Generated salt for the user: (userId: {userAccount.Id}, salt: {Convert.ToHexString(salt)})");

      _userAccountRepo.Insert(userAccount);
      _userAccountRepo.SaveChanges();

      // Asynchronously send a message of user registration

      try
      {
        var publishUserDto = _mapper.Map<PublishUserDto>(userAccount);
        publishUserDto.Event = "UserSignUp";

        var message = JsonSerializer.Serialize(publishUserDto);
        _messageBusPublisher.Publish(message);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
      }

      var userResponseDto = _mapper.Map<UserResponseDto>(userAccount);

      return Ok(
        new AuthResponseDto()
        {
          User = userResponseDto,
          IsSuccessful = true,
          JwtToken = JwtAuthHandler.GenerateToken(userAccount),
          Message = null
        }
      );
    }
  }
}
