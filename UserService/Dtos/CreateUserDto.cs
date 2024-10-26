using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos
{
  public class CreateUserDto()
  {
    [Required]
    public string Name { get; set; }
  }
}