using UserService.Models;

namespace UserService.Data
{
  public interface IUserRepo
  {
    IEnumerable<User> GetAll();

    User GetById(Guid id);

    void Insert(User user);

    void Update(User user);

    void Delete(Guid id);

    bool Save();
  }
}