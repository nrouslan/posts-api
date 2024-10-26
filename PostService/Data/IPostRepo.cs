using PostService.Models;

namespace PostService.Data
{
  public interface IPostRepo
  {
    // Users

    IEnumerable<User> GetAllUsers();

    bool IsUserExists(int id);

    bool IsExternalUserExists(int externalUserId);

    void CreateUser(User user);

    // Posts

    IEnumerable<Post> GetPostsForUser(int userId);

    Post GetPost(int userId, int postId);

    void CreatePost(int userId, Post post);

    bool SaveChanges();
  }
}