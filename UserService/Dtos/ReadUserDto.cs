namespace UserService.Dtos
{
  public class ReadUserDto()
  {
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }
  }
}