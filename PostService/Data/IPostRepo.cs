using PostService.Models;

namespace PostService.Data
{
  public interface IPostRepo
  {
    IEnumerable<Post> GetPostsForUser(Guid userId);

    Post GetPostById(Guid postId);

    void Insert(Post post);

    void Update(Post post);

    void Delete(Guid postId);

    bool SaveChanges();
  }
}