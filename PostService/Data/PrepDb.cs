using PostService.Models;

namespace PostService.Data
{
  public static class PrepDb
  {
    public static void PrepPopulation(IApplicationBuilder app, bool isProd)
    {
      using (var serviceScope = app.ApplicationServices.CreateScope())
      {
        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
      }
    }

    private static void SeedData(AppDbContext context, bool isProd)
    {
      if (!context.Posts.Any())
      {
        Console.WriteLine("--> Seeding Data...");

        context.Posts.AddRange(
            new Post() { UserId = 1, Title = "Post Title #1", Content = "Post Content #1" },
            new Post() { UserId = 2, Title = "Post Title #2", Content = "Post Content #2", },
            new Post() { UserId = 3, Title = "Post Title #3", Content = "Post Content #3", }
        );

        context.SaveChanges();
      }
      else
      {
        Console.WriteLine("--> We already have data");
      }
    }
  }
}