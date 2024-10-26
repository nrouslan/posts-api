using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.AsyncDataServices;
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

    private readonly IMessageBusClient _messageBusClient;

    public UsersController(
      IUserRepo repository,
      IMapper mapper,
      IMessageBusClient messageBusClient)
    {
      _repository = repository;
      _mapper = mapper;
      _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ReadUserDto>> GetUsers()
    {
      Console.WriteLine("--> Getting Users...");

      var users = _repository.GetAll();

      return Ok(_mapper.Map<IEnumerable<ReadUserDto>>(users));
    }

    [HttpGet("{id}", Name = nameof(GetUserById))]
    public ActionResult<ReadUserDto> GetUserById(int id)
    {
      Console.WriteLine("--> Getting a User by Id...");

      var user = _repository.GetUserById(id);

      if (user != null)
      {
        return Ok(_mapper.Map<ReadUserDto>(user));
      }

      return NotFound();
    }

    [HttpPost]
    public ActionResult<IEnumerable<ReadUserDto>> CreateUser(CreateUserDto createUserDto)
    {
      Console.WriteLine("--> Creating a User...");

      var user = _mapper.Map<User>(createUserDto);

      _repository.Insert(user);

      _repository.SaveChanges();

      var readUserDto = _mapper.Map<ReadUserDto>(user);

      try
      {
        var publishUserDto = _mapper.Map<PublishUserDto>(readUserDto);

        publishUserDto.Event = "Publish_User";

        _messageBusClient.PublishNewUser(publishUserDto);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
      }

      return CreatedAtRoute(nameof(GetUserById), new { readUserDto.Id }, readUserDto);
    }
  }
}