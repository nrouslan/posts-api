using System.Security.Claims;
using PostService.Models;

namespace PostService.Data
{
  public interface IPrincipalHelper
  {
    Task<User?> ToUser(ClaimsPrincipal principal);
  }
}