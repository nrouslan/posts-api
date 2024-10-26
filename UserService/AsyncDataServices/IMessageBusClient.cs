namespace UserService.AsyncDataServices
{
  public interface IMessageBusClient
  {
    void PublishNewUser(PublishUserDto publishUserDto);
  }
}