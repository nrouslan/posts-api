using PostService.Models;
using PostService.SyncDataServices.Grpc;

namespace PostService.Data
{
  public static class PrepDb
  {
    public static void PrepPopulation(IApplicationBuilder app, bool isProd)
    {
      using (var serviceScope = app.ApplicationServices.CreateScope())
      {
        var grpcClient = serviceScope.ServiceProvider.GetService<IUserDataClient>();

        var users = grpcClient.ReturnAllUsers();

        SeedData(serviceScope.ServiceProvider.GetService<IPostRepo>(), users);
      }
    }

    private static void SeedData(IPostRepo repo, IEnumerable<User> users)
    {
      Console.WriteLine("--> Seeding new users...");

      foreach (var user in users)
      {
        if (!repo.IsExternalUserExists(user.ExternalId))
        {
          repo.CreateUser(user);
        }
        repo.SaveChanges();
      }
    }
  }
}