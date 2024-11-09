using UserService.Models;

namespace UserService.Data
{
  public interface IUserRepo
  {
    IEnumerable<User> GetAll();

    User GetById(int id);

    User GetByEmail(string email);

    void Insert(User user);

    void Update(User user);

    void Delete(int id);

    bool IsUserExists(string userName, string email);

    bool Save();
  }
}