using System.Security.Claims;
using PostService.Models;

namespace PostService.Data
{
  public interface IPrincipalHelper
  {
    User? ToUser(ClaimsPrincipal principal);
  }
}