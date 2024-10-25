using PostService.Models;

namespace PostService.Data
{
  class PostRepo : IPostRepo
  {
    private readonly AppDbContext _context;

    public PostRepo(AppDbContext context)
    {
      _context = context;
    }

    public IEnumerable<Post> GetAll()
    {
      return _context.Posts.ToList();
    }

    public Post GetPostById(int id)
    {
      return _context.Posts.FirstOrDefault(u => u.Id == id);
    }

    public void Insert(Post post)
    {
      if (post == null)
      {
        throw new ArgumentNullException(nameof(post));
      }

      _context.Posts.Add(post);
    }

    public bool SaveChanges()
    {
      return _context.SaveChanges() >= 0;
    }
  }
}