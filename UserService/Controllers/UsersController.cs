using AutoMapper;
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

    public UsersController(
      IUserRepo repository,
      IMapper mapper,
      IPrincipalHelper principalHelper)
    {
      _repository = repository;
      _mapper = mapper;
      _principalHelper = principalHelper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ReadUserDto>> GetAllUsers()
    {
      Console.WriteLine("--> Getting all users...");

      var users = _repository.GetAll();

      return Ok(_mapper.Map<IEnumerable<ReadUserDto>>(users));
    }

    [HttpGet("{id}", Name = nameof(GetUserById))]
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

    [Authorize]
    [HttpPut("{id}")]
    public ActionResult<ReadUserDto> UpdateUser(int id, UpdateUserDto updateUserDto)
    {
      Console.WriteLine($"--> Updating a user (userId: {id})...");

      var userInDb = _repository.GetById(id);

      if (userInDb == null)
      {
        return NotFound();
      }

      User? curUser = _principalHelper.ToUser(User);

      if (curUser == null
        || curUser.Email != userInDb.Email
        || curUser.UserName != userInDb.UserName)
      {
        return Forbid();
      }

      var user = _mapper.Map<User>(updateUserDto);

      user.Id = id;

      _repository.Update(user);

      _repository.Save();

      return Ok(_mapper.Map<ReadUserDto>(user));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public ActionResult<ReadUserDto> DeleteUser(int id)
    {
      Console.WriteLine($"--> Deleting a user (userId: {id})...");

      var userInDb = _repository.GetById(id);

      if (userInDb == null)
      {
        return NotFound();
      }

      User? curUser = _principalHelper.ToUser(User);

      if (curUser == null
        || curUser.Email != userInDb.Email
        || curUser.UserName != userInDb.UserName)
      {
        return Forbid();
      }

      _repository.Delete(id);

      _repository.Save();

      return Ok(_mapper.Map<ReadUserDto>(userInDb));
    }
  }
}