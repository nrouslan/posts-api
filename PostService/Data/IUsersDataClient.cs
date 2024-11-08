using PostService.Models;

namespace PostService.Data
{
  public interface IUsersDataClient
  {
    Task<IEnumerable<User>> GetAllUsers();
  }
}