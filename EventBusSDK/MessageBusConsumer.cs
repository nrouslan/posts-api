using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBusSDK
{
  public class MessageBusConsumer : BackgroundService
  {
    private readonly IConfiguration _configuration;

    private readonly IEventProcessor _eventProcessor;

    private IConnection _connection;

    private IModel _channel;

    private string _consumeExchangeName;

    private string _queueName;

    public MessageBusConsumer(
      IConfiguration configuration,
      IEventProcessor eventProcessor)
    {
      _configuration = configuration;
      _eventProcessor = eventProcessor;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
      Thread.Sleep(15000);

      var rabbitMqConStr = _configuration.GetConnectionString("RabbitMQ");

      if (rabbitMqConStr == null)
      {
        throw new Exception(
          "(From Subscriber) RabbitMQ connection string is not defined!");
      }

      _consumeExchangeName = _configuration["ConsumeExchangeName"];

      if (_consumeExchangeName == null)
      {
        throw new Exception(
          "(From Subscriber) ConsumeExchangeName configuration variable is not provided!");
      }

      Console.WriteLine($"--> ConsumeExchangeName: {_consumeExchangeName}");

      try
      {
        var connectionFactory = new ConnectionFactory()
        {
          Uri = new Uri(rabbitMqConStr),
          DispatchConsumersAsync = true
        };

        _connection = connectionFactory.CreateConnection();

        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: _consumeExchangeName, type: ExchangeType.Fanout);

        _queueName = _channel.QueueDeclare().QueueName;

        _channel.QueueBind(queue: _queueName,
          exchange: _consumeExchangeName,
          routingKey: string.Empty);

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

        Console.WriteLine("--> Successfully connected to the message bus! (As a Consumer)");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not connect to the Message Bus (As a Consumer): {ex.Message}");
      }

      return base.StartAsync(cancellationToken);
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
      Console.WriteLine("--> RabbitMQ Consumer Connection Shutdown!");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      stoppingToken.ThrowIfCancellationRequested();

      var consumer = new AsyncEventingBasicConsumer(_channel);

      _channel.BasicConsume(queue: _queueName,
        autoAck: true,
        consumer: consumer);

      consumer.Received += Consumer_Received;

      return Task.CompletedTask;
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
    {
      var notificationMessage = Encoding.UTF8.GetString(@event.Body.ToArray());

      Console.WriteLine($"--> Event Received! Message: {notificationMessage}");

      _eventProcessor.ProcessEvent(notificationMessage);
    }

    public override void Dispose()
    {
      Console.WriteLine("--> Message Bus Consumer Disposed!");

      if (_channel.IsOpen)
      {
        _channel.Close();
        _connection.Close();
      }

      base.Dispose();
    }
  }
}