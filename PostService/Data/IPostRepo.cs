using PostService.Models;

namespace PostService.Data
{
  public interface IPostRepo
  {
    IEnumerable<Post> GetAll();

    Post GetPostById(int id);

    void Insert(Post post);

    bool SaveChanges();
  }
}