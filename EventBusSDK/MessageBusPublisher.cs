using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace EventBusSDK
{
  public class MessageBusPublisher : IDisposable
  {
    private readonly IConfiguration _configuration;

    private readonly IConnection _connection;

    private readonly IModel _channel;

    private readonly string _publishExchangeName;

    public MessageBusPublisher(IConfiguration configuration)
    {
      _configuration = configuration;

      var rabbitMqConStr = _configuration.GetConnectionString("RabbitMQ");

      _publishExchangeName = _configuration["PublishExchangeName"];

      if (rabbitMqConStr == null)
      {
        throw new Exception(
          "(From Publisher) RabbitMQ connection string is not defined!");
      }

      if (_publishExchangeName == null)
      {
        throw new Exception(
          "(From Publisher) PublishExchangeName configuration variable is not provided!");
      }

      Console.WriteLine($"--> PublishExchangeName: {_publishExchangeName}");

      try
      {
        var connectionFactory = new ConnectionFactory()
        {
          Uri = new Uri(rabbitMqConStr),
          DispatchConsumersAsync = true
        };

        _connection = connectionFactory.CreateConnection();

        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: _publishExchangeName, type: ExchangeType.Fanout);

        Console.WriteLine("--> Successfully connected to the message bus! (As a Publisher)");

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not connect to the Message Bus (As a Publisher): {ex.Message}");
      }
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
      Console.WriteLine("--> RabbitMQ Publisher Connection Shutdown!");
    }

    public void Publish(string message)
    {
      var body = Encoding.UTF8.GetBytes(message);

      _channel.BasicPublish(exchange: _publishExchangeName,
        routingKey: string.Empty,
        basicProperties: null,
        body: body);

      Console.WriteLine($"--> We have sent the message: {message}");
    }

    public void Dispose()
    {
      Console.WriteLine("--> Message Bus Publisher Disposed!");

      if (_channel.IsOpen)
      {
        _channel.Close();
        _connection.Close();
      }
    }
  }
}