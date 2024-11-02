using UserService.Models;

namespace UserService.Data
{
  class UserRepo : IUserRepo
  {
    private readonly AppDbContext _context;

    public UserRepo(AppDbContext context)
    {
      _context = context;
    }

    public IEnumerable<User> GetAll()
    {
      return _context.Users
        .OrderBy(u => u.Id)
        .ToList();
    }

    public User GetById(int id)
    {
      return _context.Users.FirstOrDefault(u => u.Id == id);
    }

    public User GetByEmail(string email)
    {
      return _context.Users.FirstOrDefault(u => u.Email == email);
    }

    public void Insert(User user)
    {
      if (user == null)
      {
        throw new ArgumentNullException(nameof(user));
      }

      _context.Users.Add(user);
    }

    public void Update(User user)
    {
      var userInDb = _context.Users.Find(user.Id);

      if (userInDb != null)
      {
        userInDb.UserName = user.UserName;
        userInDb.Email = user.Email;
      }
    }

    public void Delete(int id)
    {
      var userInDb = _context.Users.Find(id);

      if (userInDb != null)
      {
        _context.Users.Remove(userInDb);
      }
    }

    public bool Save()
    {
      return _context.SaveChanges() >= 0;
    }
  }
}