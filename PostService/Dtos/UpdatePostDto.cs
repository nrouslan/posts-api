using System.ComponentModel.DataAnnotations;

namespace PostService.Dtos
{
  public class UpdatePostDto()
  {
    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public string PublishedAt { get; set; }
  }
}