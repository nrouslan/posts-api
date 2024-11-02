using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos
{
  public class SignInRequestDto
  {
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
  }
}