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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      UserAccount[] userAccounts = [
        new UserAccount { Id = 1, UserName = "rusnik", Email = "rusnik@gmail.com", Password = "12345678" },
        new UserAccount { Id = 2, UserName = "olepak", Email = "olepak@gmail.com", Password = "12345678" },
        new UserAccount { Id = 3, UserName = "andfom", Email = "andfom@gmail.com", Password = "12345678" }
      ];

      modelBuilder.Entity<UserAccount>().HasData(userAccounts);
    }
  }
}