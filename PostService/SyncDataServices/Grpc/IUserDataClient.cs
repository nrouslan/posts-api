using PostService.Models;

namespace PostService.SyncDataServices.Grpc
{
  public interface IUserDataClient
  {
    IEnumerable<User> ReturnAllUsers();
  }
}