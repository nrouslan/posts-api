using UserService.Dtos;

namespace UserService.Data
{
  public interface IMessageBusClient
  {
    void PublishUserDelete(PublishUserDeleteDto publishUserDeleteDto);
  }
}