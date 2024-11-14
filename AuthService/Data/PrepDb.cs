using AuthService.Models;

namespace AuthService.Data
{
  public static class PrepDb
  {
    private static UserAccount[] UserAccounts = [
        new UserAccount {
          Id = 1,
          UserName = "rusnik",
          Email = "rusnik@gmail.com",
          RegisteredAt = "2021-11-01T21:00:00Z" },
        new UserAccount {
          Id = 2,
          UserName = "olepak",
          Email = "olepak@gmail.com",
          RegisteredAt = "2021-10-01T21:00:00Z" },
        new UserAccount {
          Id = 3,
          UserName = "andfom",
          Email = "andfom@gmail.com",
          RegisteredAt = "2021-09-01T21:00:00Z" }
      ];

    public static void PrepPopulation(IApplicationBuilder app)
    {
      using (var serviceScope = app.ApplicationServices.CreateScope())
      {
        SeedData(
          serviceScope.ServiceProvider.GetService<AppDbContext>()
        );
      }
    }

    private static void SeedData(AppDbContext context)
    {
      if (!context.UsersAccounts.Any())
      {
        Console.WriteLine("--> Seeding UserAccountsDb...");

        foreach (var userAccount in UserAccounts)
        {
          userAccount.PasswordHash = PasswordHasher.HashPassword("12345678", out var salt);

          Console.WriteLine(
            $"--> Generated salt for the user: (userId: {userAccount.Id}, salt: {Convert.ToHexString(salt)})");

          userAccount.PasswordSalt = Convert.ToHexString(salt);

          context.UsersAccounts.Add(userAccount);
        }

        context.SaveChanges();

        Console.WriteLine(
          $"--> Added {UserAccounts.Count()} user accounts to the database!");
      }
      else
      {
        Console.WriteLine("--> We already have data!");
      }
    }
  }
}