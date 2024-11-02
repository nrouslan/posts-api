namespace AuthService.Dtos
{
  public class AuthResponseDto
  {
    public string UserName { get; set; }

    public string Email { get; set; }

    public string JwtToken { get; set; }

    public bool IsSuccessful { get; set; }

    public string Message { get; set; }
  }
}