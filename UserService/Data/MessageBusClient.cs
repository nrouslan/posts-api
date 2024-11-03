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

      if (_connection.IsOpen)
      {
        Console.WriteLine("--> RabbitMQ Connection is opened, sending the message...");

        SendMessage(message);
      }
      else
      {
        Console.WriteLine("--> RabbitMQ Connection is closed, not sending the message");
      }
    }
  }
}