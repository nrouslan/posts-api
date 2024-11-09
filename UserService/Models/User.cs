using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace UserService.Models
{
  [Index(nameof(UserName), IsUnique = true)]
  [Index(nameof(Email), IsUnique = true)]

  public class User
  {
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string RegisteredAt { get; set; }
  }
}