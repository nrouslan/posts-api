using PostService.Models;

namespace PostService.Data
{
  public interface IPostRepo
  {
    IEnumerable<Post> GetPostsForUser(int userId);

    Post GetPostById(int userId, int postId);

    void Insert(Post post);

    void Update(int userId, Post post);

    void Delete(int userId, int postId);

    bool SaveChanges();
  }
}