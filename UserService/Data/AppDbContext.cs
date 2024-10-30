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
        new () { Id = Guid.NewGuid(), UserName = "rusnik", Email = "rusnik@gmail.com" },
        new () { Id = Guid.NewGuid(), UserName = "olepak", Email = "olepak@gmail.com" },
        new () { Id = Guid.NewGuid(), UserName = "andfom", Email = "andfom@gmail.com" },
      ];

      modelBuilder.Entity<User>().HasData(users);
    }
  }
}