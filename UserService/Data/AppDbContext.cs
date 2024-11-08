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
        new () { Id = 1, UserName = "rusnik", Email = "rusnik@gmail.com", RegisteredAt = "2021-11-01T21:00:00Z" },
        new () { Id = 2, UserName = "olepak", Email = "olepak@gmail.com", RegisteredAt = "2021-10-01T21:00:00Z" },
        new () { Id = 3, UserName = "andfom", Email = "andfom@gmail.com", RegisteredAt = "2021-09-01T21:00:00Z" },
      ];

      modelBuilder.Entity<User>().HasData(users);
    }
  }
}