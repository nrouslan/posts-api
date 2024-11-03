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

      _userAccountRepo.Insert(userAccount);

      _userAccountRepo.SaveChanges();

      // TODO: RabbitMQ Message (User Sign Up)

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
