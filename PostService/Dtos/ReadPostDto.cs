namespace PostService.Dtos
{
  public class ReadPostDto()
  {
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public string PublishedAt { get; set; }
  }
}