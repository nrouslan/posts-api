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

    public void CreatePost(int userId, Post post)
    {
      if (post == null)
      {
        throw new ArgumentNullException(nameof(post));
      }

      // Set user id passed from URL

      post.UserId = userId;

      _context.Posts.Add(post);
    }

    public void CreateUser(User user)
    {
      if (user == null)
      {
        throw new ArgumentNullException(nameof(user));
      }

      _context.Users.Add(user);
    }

    public IEnumerable<User> GetAllUsers()
    {
      return _context.Users.ToList();
    }

    public Post GetPost(int userId, int postId)
    {
      return _context.Posts
        .Where(p => p.UserId == userId && p.Id == postId)
        .FirstOrDefault();
    }

    public IEnumerable<Post> GetPostsForUser(int userId)
    {
      return _context.Posts
        .Where(p => p.UserId == userId)
        .OrderBy(p => p.Title);
    }

    public bool IsUserExists(int id)
    {
      return _context.Users.Any(u => u.Id == id);
    }

    public bool IsExternalUserExists(int externalUserId)
    {
      return _context.Users.Any(u => u.ExternalId == externalUserId);
    }

    public bool SaveChanges()
    {
      return _context.SaveChanges() >= 0;
    }
  }
}