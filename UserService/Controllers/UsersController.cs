using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly IUserRepo _repository;

    private readonly IMapper _mapper;

    public UsersController(
      IUserRepo repository,
      IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ReadUserDto>> GetAllUsers()
    {
      Console.WriteLine("--> Getting All Users...");

      var users = _repository.GetAll();

      return Ok(_mapper.Map<IEnumerable<ReadUserDto>>(users));
    }

    [HttpGet("{id:guid}", Name = nameof(GetUserById))]
    public ActionResult<ReadUserDto> GetUserById(Guid id)
    {
      Console.WriteLine("--> Getting a User by Id...");

      var user = _repository.GetById(id);

      if (user != null)
      {
        return Ok(_mapper.Map<ReadUserDto>(user));
      }

      return NotFound();
    }

    [HttpPost]
    public ActionResult<ReadUserDto> CreateUser(CreateUserDto createUserDto)
    {
      Console.WriteLine("--> Creating a User...");

      var user = _mapper.Map<User>(createUserDto);

      _repository.Insert(user);

      _repository.Save();

      var readUserDto = _mapper.Map<ReadUserDto>(user);

      return CreatedAtRoute(nameof(GetUserById), new { readUserDto.Id }, readUserDto);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<ReadUserDto> UpdateUser(Guid id, UpdateUserDto updateUserDto)
    {
      Console.WriteLine("--> Updating a User...");

      var userInDb = _repository.GetById(id);

      if (userInDb == null)
      {
        return NotFound();
      }

      var user = _mapper.Map<User>(updateUserDto);

      user.Id = id;

      _repository.Update(user);

      _repository.Save();

      return Ok(_mapper.Map<ReadUserDto>(user));
    }

    [HttpDelete("{id:guid}")]
    public ActionResult<ReadUserDto> DeleteUser(Guid id)
    {
      Console.WriteLine("--> Deleting a User...");

      var user = _repository.GetById(id);

      if (user == null)
      {
        return NotFound();
      }

      _repository.Delete(id);

      _repository.Save();

      return Ok(_mapper.Map<ReadUserDto>(user));
    }
  }
}