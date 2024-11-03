using PostService.Models;

namespace PostService.Data
{
  public interface IUsersDataClient
  {
    Task<User?> GetUserById(int id);
  }
}