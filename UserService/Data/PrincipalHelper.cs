using System.Security.Claims;
using UserService.Models;

namespace UserService.Data
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
      Claim? nameIdentifier = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

      if (email == null)
      {
        return null;
      }

      User? user = _userRepo.GetByEmail(email.Value);

      return user;
    }
  }
}