using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
  public class AppDbContext : DbContext
  {
    public DbSet<UserAccount> UsersAccounts { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {
      Database.EnsureCreated();
    }
  }
}