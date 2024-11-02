using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Auth
{
  public static class JwtAuthHandler
  {
    public static string GenerateToken(UserAccount user)
    {
      var claims = new Claim[] {
        new(ClaimTypes.Name, user.UserName),
        new(ClaimTypes.Email, user.Email)
      };

      var jwt = new JwtSecurityToken(
        claims: claims,
        signingCredentials: new SigningCredentials(
          JwtAuthOptions.GetSymmetricSecurityKey(),
          algorithm: SecurityAlgorithms.HmacSha256
        )
      );

      return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
  }
}