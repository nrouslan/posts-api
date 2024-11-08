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

    public IEnumerable<Post> GetPostsForUser(int userId)
    {
      return _context.Posts
        .Where(p => p.UserId == userId)
        .OrderBy(p => p.Id)
        .ToList();
    }

    public Post GetPostById(int userId, int postId)
    {
      return _context.Posts
        .Where(p => p.UserId == userId && p.Id == postId)
        .FirstOrDefault();
    }

    public void Insert(Post post)
    {
      if (post == null)
      {
        throw new ArgumentNullException(nameof(post));
      }

      _context.Posts.Add(post);
    }

    public void Update(int userId, Post post)
    {
      if (post == null)
      {
        throw new ArgumentNullException(nameof(post));
      }

      var postInDb = _context.Posts.FirstOrDefault(
        p => p.UserId == userId && p.Id == post.Id
      );

      if (postInDb != null)
      {
        postInDb.Title = post.Title;
        postInDb.Content = post.Content;
      }
    }

    public void Delete(int userId, int postId)
    {
      var postInDb = _context.Posts.FirstOrDefault(
        p => p.UserId == userId && p.Id == postId
      );

      if (postInDb != null)
      {
        _context.Posts.Remove(postInDb);
      }
    }

    public bool SaveChanges()
    {
      return _context.SaveChanges() >= 0;
    }
  }
}