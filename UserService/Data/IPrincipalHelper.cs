using System.Security.Claims;
using UserService.Models;

namespace UserService.Data
{
  public interface IPrincipalHelper
  {
    User? ToUser(ClaimsPrincipal principal);
  }
}