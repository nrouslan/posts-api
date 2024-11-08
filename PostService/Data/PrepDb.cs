namespace PostService.Data
{
  public static class PrepDb
  {
    public static async Task PrepPopulation(IApplicationBuilder app)
    {
      using (var serviceScope = app.ApplicationServices.CreateScope())
      {
        await SeedData(
          serviceScope.ServiceProvider.GetService<AppDbContext>(),
          serviceScope.ServiceProvider.GetService<IUsersDataClient>()
        );
      }
    }

    private static async Task SeedData(
      AppDbContext context,
      IUsersDataClient usersDataClient)
    {
      if (!context.Users.Any())
      {
        Console.WriteLine("--> Fetching user's data...");

        var users = await usersDataClient.GetAllUsers();

        context.Users.AddRange(users);

        context.SaveChanges();

        Console.WriteLine($"--> Added {users.Count()} users to the Db!");
      }
      else
      {
        Console.WriteLine("--> We already have data");
      }
    }
  }
}