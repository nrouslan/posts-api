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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // Define composite key
      modelBuilder.Entity<Post>()
        .HasKey(p => new { p.Id, p.UserId });

      // Auto-increment id
      modelBuilder.Entity<Post>()
        .Property(p => p.Id).ValueGeneratedOnAdd();

      Post[] posts = [
        new () { Id = 1, UserId = 1, Title = "User 1 Post 1 Title", Content = "User 1 Post 1 Content", PublishedAt = "2024-11-01T23:00:00Z" },
        new () { Id = 2, UserId = 1, Title = "User 1 Post 2 Title", Content = "User 1 Post 2 Content", PublishedAt = "2024-10-01T23:00:00Z" },
        new () { Id = 3, UserId = 1, Title = "User 1 Post 3 Title", Content = "User 1 Post 3 Content", PublishedAt = "2024-9-01T23:00:00Z" },
        new () { Id = 1, UserId = 2, Title = "User 2 Post 1 Title", Content = "User 2 Post 1 Content", PublishedAt = "2023-11-01T23:00:00Z" },
        new () { Id = 2, UserId = 2, Title = "User 2 Post 2 Title", Content = "User 2 Post 2 Content", PublishedAt = "2023-10-01T23:00:00Z" },
        new () { Id = 3, UserId = 2, Title = "User 2 Post 3 Title", Content = "User 2 Post 3 Content", PublishedAt = "2023-9-01T23:00:00Z" },
        new () { Id = 1, UserId = 3, Title = "User 3 Post 1 Title", Content = "User 3 Post 1 Content", PublishedAt = "2022-11-01T23:00:00Z" },
        new () { Id = 2, UserId = 3, Title = "User 3 Post 2 Title", Content = "User 3 Post 2 Content", PublishedAt = "2022-10-01T23:00:00Z" },
        new () { Id = 3, UserId = 3, Title = "User 3 Post 3 Title", Content = "User 3 Post 3 Content", PublishedAt = "2022-9-01T23:00:00Z" },
      ];

      modelBuilder.Entity<Post>().HasData(posts);
    }
  }
}