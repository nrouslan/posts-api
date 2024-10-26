using UserService.Models;

namespace UserService.Data
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
      if (!context.Users.Any())
      {
        Console.WriteLine("--> Seeding Data...");

        context.Users.AddRange(
            new User() { Name = "Ruslan", },
            new User() { Name = "Danil", },
            new User() { Name = "Anechka", }
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