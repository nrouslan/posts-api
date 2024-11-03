using AuthService.Models;

namespace AuthService.Data
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

    public bool IsUserAccountExist(string userName, string email)
    {
      return _context.UsersAccounts.Any(ua =>
        ua.UserName == userName || ua.Email == email);
    }

    public void Insert(UserAccount userAccount)
    {
      if (userAccount == null)
      {
        throw new ArgumentNullException(nameof(userAccount));
      }

      _context.UsersAccounts.Add(userAccount);
    }

    public void Update(UserAccount userAccount)
    {
      var userAccountrInDb = _context.UsersAccounts.Find(userAccount.Id);

      if (userAccountrInDb != null)
      {
        userAccountrInDb.UserName = userAccount.UserName;
        userAccountrInDb.Email = userAccount.Email;
      }
    }

    public void Delete(int id)
    {
      var userAccountInDb = _context.UsersAccounts.Find(id);

      if (userAccountInDb != null)
      {
        _context.UsersAccounts.Remove(userAccountInDb);
      }
    }

    public bool SaveChanges()
    {
      return _context.SaveChanges() >= 0;
    }
  }
}