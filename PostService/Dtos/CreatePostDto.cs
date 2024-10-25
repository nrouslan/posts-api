namespace PostService.Dtos
{
  public class CreatePostDto()
  {
    public int UserId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }
  }
}