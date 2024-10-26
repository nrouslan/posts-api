using System.ComponentModel.DataAnnotations;

namespace PostService.Models
{
  public class User()
  {
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public int ExternalId { get; set; }

    public ICollection<Post> Posts { get; set; } = new List<Post>() { };

    [Required]
    public string Name { get; set; }
  }
}