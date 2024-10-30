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

    public IEnumerable<Post> GetPostsForUser(Guid userId)
    {
      return _context.Posts
        .Where(p => p.UserId == userId)
        .OrderBy(p => p.Title)
        .ToList();
    }

    public Post GetPostById(Guid postId)
    {
      return _context.Posts
        .Where(p => p.Id == postId)
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

    public void Update(Post post)
    {
      var postInDb = _context.Posts.Find(post.Id);

      if (postInDb != null)
      {
        postInDb.Title = post.Title;
        postInDb.Content = post.Content;
        postInDb.PublishedAt = post.PublishedAt;
      }
    }

    public void Delete(Guid postId)
    {
      var postInDb = _context.Posts.Find(postId);

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