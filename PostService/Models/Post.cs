using System.ComponentModel.DataAnnotations;

namespace PostService.Models
{
  public class Post()
  {
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public string PublishedAt { get; set; }
  }
}