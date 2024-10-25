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
      return _context.Users.ToList();
    }

    public User GetUserById(int id)
    {
      return _context.Users.FirstOrDefault(u => u.Id == id);
    }

    public void Insert(User user)
    {
      if (user == null)
      {
        throw new ArgumentNullException(nameof(user));
      }

      _context.Users.Add(user);
    }

    public bool SaveChanges()
    {
      return _context.SaveChanges() >= 0;
    }
  }
}