using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
  public class User()
  {
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    public string Email { get; set; }
  }
}