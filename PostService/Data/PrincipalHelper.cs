using System.Security.Claims;
using PostService.Models;

namespace PostService.Data
{
  public class PrincipalHelper : IPrincipalHelper
  {
    private readonly IUsersDataClient _usersDataClient;

    public PrincipalHelper(IUsersDataClient usersDataClient)
    {
      _usersDataClient = usersDataClient;
    }

    public async Task<User?> ToUser(ClaimsPrincipal principal)
    {
      Claim? nameIdentifier = principal.Claims.FirstOrDefault(
        c => c.Type == ClaimTypes.NameIdentifier);

      if (nameIdentifier == null)
      {
        return null;
      }

      return await _usersDataClient.GetUserById(
        int.Parse(nameIdentifier.Value));
    }
  }
}