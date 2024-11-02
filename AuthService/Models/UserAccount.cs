using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Models
{
  [Index(nameof(UserName), IsUnique = true)]
  public class UserAccount
  {
    [Required]
    public string UserName { get; set; }

    [Key]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
  }
}