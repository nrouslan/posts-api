using UserService.Models;

namespace UserService.Data
{
  public interface IUserRepo
  {
    IEnumerable<User> GetAll();

    User GetUserById(int id);

    void Insert(User user);

    bool SaveChanges();
  }
}