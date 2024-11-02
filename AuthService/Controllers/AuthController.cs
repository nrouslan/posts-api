using AuthService.Auth;
using AuthService.Data;
using AuthService.Dtos;
using AuthService.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly IUserAccountRepo _userAccountRepo;

    private readonly IMapper _mapper;

    public AuthController(
      IUserAccountRepo userAccountRepo,
      IMapper mapper)
    {
      _userAccountRepo = userAccountRepo;
      _mapper = mapper;
    }

    [HttpPost("signin")]
    public ActionResult<AuthResponseDto> SignUserIn(
      [FromBody] SignInRequestDto signInRequestDto)
    {
      Console.WriteLine("--> Signing in a user...");

      var userAccount = _userAccountRepo.GetUserAccountByEmail(signInRequestDto.Email);

      if (userAccount == null
        || userAccount.Email != signInRequestDto.Email
        || userAccount.Password != signInRequestDto.Password)
      {
        return Ok(
          new AuthResponseDto()
          {
            IsSuccessful = false,
            UserName = null,
            Email = null,
            JwtToken = null,
            Message = "Неверная почта и/или пароль!"
          }
        );
      }

      return Ok(
        new AuthResponseDto()
        {
          IsSuccessful = true,
          UserName = userAccount.UserName,
          Email = userAccount.Email,
          JwtToken = JwtAuthHandler.GenerateToken(userAccount),
          Message = null
        }
      );
    }

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
            IsSuccessful = false,
            UserName = null,
            Email = null,
            JwtToken = null,
            Message = "Почта и/или имя пользователя уже заняты!"
          }
        );
      }

      var userAccount = _mapper.Map<UserAccount>(signUpRequestDto);

      _userAccountRepo.Insert(userAccount);

      _userAccountRepo.SaveChanges();

      // TODO: RabbitMQ Message (User Sign Up)

      return Ok(
        new AuthResponseDto()
        {
          IsSuccessful = true,
          UserName = userAccount.UserName,
          Email = userAccount.Email,
          JwtToken = JwtAuthHandler.GenerateToken(userAccount),
          Message = null
        }
      );
    }
  }
}
