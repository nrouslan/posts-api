namespace AuthService.Dtos
{
  public class PublishUserDto
  {
    public int Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string RegisteredAt { get; set; }

    public string Event { get; set; }
  }
}