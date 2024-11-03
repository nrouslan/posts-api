using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Models
{
  [Index(nameof(UserName), IsUnique = true)]
  [Index(nameof(Email), IsUnique = true)]
  public class UserAccount
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
  }
}