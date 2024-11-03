namespace AuthService.Dtos
{
  public class AuthResponseDto
  {
    public UserResponseDto User { get; set; }

    public string JwtToken { get; set; }

    public bool IsSuccessful { get; set; }

    public string Message { get; set; }
  }
}