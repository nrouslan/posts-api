using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data
{
  public class AppDbContext : DbContext
  {
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {
      Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      User[] users = [
        new () { Id = 1, UserName = "rusnik", Email = "rusnik@gmail.com" },
        new () { Id = 2, UserName = "olepak", Email = "olepak@gmail.com" },
        new () { Id = 3, UserName = "andfom", Email = "andfom@gmail.com" },
      ];

      modelBuilder.Entity<User>().HasData(users);
    }
  }
}