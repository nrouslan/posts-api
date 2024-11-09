using PostService.Models;

namespace PostService.Data
{
  public interface IUserRepo
  {
    User? GetUserById(int userId);

    void Insert(User user);

    void Update(User user);

    void Delete(int userId);

    bool SaveChanges();
  }
}