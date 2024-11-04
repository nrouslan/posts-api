using System.Text;
using RabbitMQ.Client;

namespace EventBusSDK
{
  public class MessageBusPublisher
  {
    private readonly MessageBusClientService _messageBusClientService;

    public MessageBusPublisher(MessageBusClientService messageBusClientService)
    {
      _messageBusClientService = messageBusClientService;
    }

    public void Publish(string message)
    {
      var channel = _messageBusClientService.Connect();

      var body = Encoding.UTF8.GetBytes(message);

      var properties = channel.CreateBasicProperties();

      properties.Persistent = true;

      channel.BasicPublish(
        exchange: MessageBusClientService.ExchangeName,
        routingKey: MessageBusClientService.RoutingPostsApi,
        basicProperties: properties,
        body: body);

      Console.WriteLine($"--> We have sent the message: {message}");
    }
  }
}