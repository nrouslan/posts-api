namespace PostService.Dtos
{
  public class ReadPostDto()
  {
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public string PublishedAt { get; set; }
  }
}