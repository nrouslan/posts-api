using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PostService.Data;
using PostService.Dtos;
using PostService.Models;

namespace PostService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PostsController : ControllerBase
  {
    private readonly IPostRepo _repository;
    private readonly IMapper _mapper;

    public PostsController(IPostRepo repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ReadPostDto>> GetPosts()
    {
      Console.WriteLine("--> Getting Posts...");

      var posts = _repository.GetAll();

      return Ok(_mapper.Map<IEnumerable<ReadPostDto>>(posts));
    }

    [HttpGet("{id}", Name = nameof(GetPostById))]
    public ActionResult<ReadPostDto> GetPostById(int id)
    {
      Console.WriteLine("--> Getting a Post by Id...");

      var post = _repository.GetPostById(id);

      if (post != null)
      {
        return Ok(_mapper.Map<ReadPostDto>(post));
      }

      return NotFound();
    }

    [HttpPost]
    public ActionResult<IEnumerable<ReadPostDto>> CreatePost(CreatePostDto createPostDto)
    {
      Console.WriteLine("--> Creating a Post...");

      var post = _mapper.Map<Post>(createPostDto);

      _repository.Insert(post);

      _repository.SaveChanges();

      var readPostDto = _mapper.Map<ReadPostDto>(post);

      return CreatedAtRoute(nameof(GetPostById), new { readPostDto.Id }, readPostDto);
    }
  }
}