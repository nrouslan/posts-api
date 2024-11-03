using System.Text.Json;
using AuthService.Dtos;
using EventBusSDK;

namespace AuthService.Data
{
  public class MessageBusClient : MessageBusClientBase, IMessageBusClient
  {
    public MessageBusClient(IConfiguration configuration) : base(configuration) { }

    public void PublishNewUser(PublishUserDto publishUserDto)
    {
      var message = JsonSerializer.Serialize(publishUserDto);

      SendMessage(message);
    }
  }
}