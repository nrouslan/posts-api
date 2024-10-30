using Microsoft.EntityFrameworkCore;
using PostService.Models;

namespace PostService.Data
{
  public class AppDbContext : DbContext
  {
    public DbSet<Post> Posts { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {
      Database.EnsureCreated();
    }
  }
}