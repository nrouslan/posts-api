using System.ComponentModel.DataAnnotations;

namespace PostService.Models
{
  public class Post()
  {
    [Key]
    [Required]
    public int Id { get; set; }

    [Key]
    [Required]
    public int UserId { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public string PublishedAt { get; set; }
  }
}