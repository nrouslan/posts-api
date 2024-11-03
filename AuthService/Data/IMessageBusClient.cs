using AuthService.Dtos;

namespace AuthService.Data
{
  public interface IMessageBusClient
  {
    void PublishNewUser(PublishUserDto publishUserDto);
  }
}