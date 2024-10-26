using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PostService.Data;
using PostService.Dtos;
using PostService.Models;

namespace PostService.Controllers
{
  [Route("p/api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly IPostRepo _repository;
    private readonly IMapper _mapper;

    public UsersController(IPostRepo repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ReadUserDto>> GetUsers()
    {
      Console.WriteLine("--> Getting Users from PostService...");

      var users = _repository.GetAllUsers();

      return Ok(_mapper.Map<IEnumerable<ReadUserDto>>(users));
    }
  }
}