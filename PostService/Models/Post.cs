using System.ComponentModel.DataAnnotations;

namespace PostService.Models
{
  public class Post()
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }
  }
}