using System.Text;
using System.Text.Json;
using AuthService.Dtos;
using RabbitMQ.Client;

namespace AuthService.Data
{
  public class MessageBusClient : IMessageBusClient
  {
    private readonly IConfiguration _configuration;

    private readonly IConnection _connection;

    private readonly IModel _channel;

    public MessageBusClient(IConfiguration configuration)
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

    private void SendMessage(string message)
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

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
      Console.WriteLine("--> RabbitMQ Connection Shutdown");
    }
  }
}