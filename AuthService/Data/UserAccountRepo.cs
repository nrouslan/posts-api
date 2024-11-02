using AuthService.Data;
using AuthService.Models;

namespace UserService.Data
{
  class UserAccountRepo : IUserAccountRepo
  {
    private readonly AppDbContext _context;

    public UserAccountRepo(AppDbContext context)
    {
      _context = context;
    }

    public UserAccount GetUserAccountByEmail(string email)
    {
      return _context.UsersAccounts.FirstOrDefault(ua => ua.Email == email);
    }

    public void Insert(UserAccount userAccount)
    {
      if (userAccount == null)
      {
        throw new ArgumentNullException(nameof(userAccount));
      }

      _context.UsersAccounts.Add(userAccount);
    }

    public bool IsUserAccountExist(string userName, string email)
    {
      return _context.UsersAccounts.Any(ua =>
        ua.UserName == userName || ua.Email == email);
    }

    public bool SaveChanges()
    {
      return _context.SaveChanges() >= 0;
    }
  }
}