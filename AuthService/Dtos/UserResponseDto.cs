namespace AuthService.Dtos
{
  public class UserResponseDto
  {
    public int Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string RegisteredAt { get; set; }
  }
}