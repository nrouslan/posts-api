using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos
{
  public class UpdateUserDto
  {
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Email { get; set; }
  }
}