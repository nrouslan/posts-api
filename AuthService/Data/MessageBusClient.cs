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