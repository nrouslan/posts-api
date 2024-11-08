using System.Security.Claims;
using PostService.Models;

namespace PostService.Data
{
  public class PrincipalHelper : IPrincipalHelper
  {
    private readonly IUserRepo _userRepo;

    public PrincipalHelper(IUserRepo userRepo)
    {
      _userRepo = userRepo;
    }

    public User? ToUser(ClaimsPrincipal principal)
    {
      Claim? nameIdentifier = principal.Claims.FirstOrDefault(
        c => c.Type == ClaimTypes.NameIdentifier);

      if (nameIdentifier == null)
      {
        return null;
      }

      return _userRepo.GetUserById(
        int.Parse(nameIdentifier.Value));
    }
  }
}