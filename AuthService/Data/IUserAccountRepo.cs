using AuthService.Models;

namespace AuthService.Data
{
  public interface IUserAccountRepo
  {
    bool IsUserAccountExist(string userName, string email);

    UserAccount GetUserAccountByEmail(string email);

    void Insert(UserAccount user);

    void Update(UserAccount user);

    void Delete(int id);

    bool SaveChanges();
  }
}