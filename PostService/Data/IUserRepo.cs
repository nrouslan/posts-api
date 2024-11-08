using PostService.Models;

namespace PostService.Data
{
  public interface IUserRepo
  {
    User? GetUserById(int userId);

    void Update(User user);

    void Delete(int userId);

    bool SaveChanges();
  }
}