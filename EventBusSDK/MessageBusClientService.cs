using RabbitMQ.Client;

namespace EventBusSDK
{
  public class MessageBusClientService : IDisposable
  {
    private readonly ConnectionFactory _connectionFactory;

    private IConnection _connection;

    private IModel _channel;

    public static string ExchangeName = "PostsApiExchange";

    public static string RoutingPostsApi = "PostsApiRoute";

    public static string QueueName = "PostsApiQueue";

    public MessageBusClientService(ConnectionFactory connectionFactory)
    {
      _connectionFactory = connectionFactory;
    }

    public IModel Connect()
    {
      _connection = _connectionFactory.CreateConnection();

      if (_channel is { IsOpen: true })
      {
        return _channel;
      }

      _channel = _connection.CreateModel();

      _channel.ExchangeDeclare(ExchangeName, type: ExchangeType.Direct, true, false);

      _channel.QueueDeclare(QueueName, true, false, false, null);

      _channel.QueueBind(exchange: ExchangeName, queue: QueueName, routingKey: RoutingPostsApi);

      Console.WriteLine("--> Connected to RabbitMQ successfuly...");

      return _channel;
    }

    public void Dispose()
    {
      Console.WriteLine("--> Message Bus disposed.");

      if (_channel.IsOpen)
      {
        _channel?.Close();
        _channel?.Dispose();

        _connection?.Close();
        _connection?.Dispose();
      }
    }
  }
}