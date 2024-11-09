using System.ComponentModel.DataAnnotations;

namespace PostService.Dtos
{
  public class CreatePostDto()
  {
    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }
  }
}