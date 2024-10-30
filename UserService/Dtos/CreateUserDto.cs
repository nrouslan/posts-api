using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos
{
  public class CreateUserDto()
  {
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Email { get; set; }
  }
}