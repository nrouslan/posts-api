using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace EventBusSDK
{
  public class MessageBusClientBase
  {
    protected readonly IConfiguration _configuration;

    protected readonly IConnection _connection;

    protected readonly IModel _channel;

    public MessageBusClientBase(IConfiguration configuration)
    {
      _configuration = configuration;

      var factory = new ConnectionFactory()
      {
        HostName = _configuration["RabbitMQHost"],
        Port = int.Parse(_configuration["RabbitMQPort"])
      };

      try
      {
        _connection = factory.CreateConnection();

        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

        Console.WriteLine("--> Connected to the Message Bus");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
      }
    }

    protected void SendMessage(string message)
    {
      var body = Encoding.UTF8.GetBytes(message);

      _channel.BasicPublish(
        exchange: "trigger",
        routingKey: "",
        basicProperties: null,
        body: body);

      Console.WriteLine($"--> We have sent the message: {message}");
    }

    public void Dispose()
    {
      Console.WriteLine("--> Message Bus disposed.");

      if (_channel.IsOpen)
      {
        _channel.Close();
        _connection.Close();
      }
    }

    protected void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
      Console.WriteLine("--> RabbitMQ Connection Shutdown");
    }
  }
}