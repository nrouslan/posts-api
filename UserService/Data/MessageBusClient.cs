using System.Text.Json;
using UserService.Dtos;
using EventBusSDK;

namespace UserService.Data
{
  public class MessageBusClient : MessageBusClientBase, IMessageBusClient
  {
    public MessageBusClient(IConfiguration configuration) : base(configuration) { }

    public void PublishUserDelete(PublishUserDeleteDto publishUserDeleteDto)
    {
      var message = JsonSerializer.Serialize(publishUserDeleteDto);

      SendMessage(message);
    }

    public void PublishUserUpdate(PublishUserUpdateDto publishUserUpdateDto)
    {
      var message = JsonSerializer.Serialize(publishUserUpdateDto);

      SendMessage(message);
    }
  }
}