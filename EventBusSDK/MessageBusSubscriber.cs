using System.Text;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBusSDK
{
  public class MessageBusSubscriber : BackgroundService
  {
    private readonly MessageBusClientService _messageBusClientService;

    private IModel _channel;

    private readonly IEventProcessor _eventProcessor;

    public MessageBusSubscriber(
      MessageBusClientService messageBusClientService,
      IEventProcessor eventProcessor)
    {
      _messageBusClientService = messageBusClientService;
      _eventProcessor = eventProcessor;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
      Thread.Sleep(30000);

      _channel = _messageBusClientService.Connect();

      _channel.BasicQos(0, 1, false);

      return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var consumer = new AsyncEventingBasicConsumer(_channel);

      _channel.BasicConsume(MessageBusClientService.QueueName, false, consumer);

      consumer.Received += Consumer_Received;
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
    {
      var message = Encoding.UTF8.GetString(@event.Body.ToArray());

      Console.WriteLine($"--> Received the message: {message}");

      _eventProcessor.ProcessEvent(message);

      _channel.BasicAck(@event.DeliveryTag, false);
    }
  }
}