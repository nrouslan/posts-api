using UserService.Models;

namespace UserService.Data
{
  public interface IUserRepo
  {
    IEnumerable<User> GetAll();

    User GetById(int id);

    void Insert(User user);

    void Update(User user);

    void Delete(int id);

    bool Save();
  }
}