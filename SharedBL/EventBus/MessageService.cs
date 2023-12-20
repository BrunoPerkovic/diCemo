using System.Text;
using RabbitMQ.Client;

namespace SharedBL.EventBus;

public class MessageService : IMessageService
{
    private readonly ConnectionFactory _factory;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageService(ConnectionFactory factory, IConnection connection, IModel channel)
    {
        Console.WriteLine("about to connect to rabbit");

        _factory = new ConnectionFactory
        {
            HostName = "rabbitmq", Port = 5672,
            UserName = "guest",
            Password = "guest"
        };
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "hello",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public bool Enqueue(string message)
    {
        var body = Encoding.UTF8.GetBytes("server processed " + message);
        _channel.BasicPublish(exchange: "",
            routingKey: "hello",
            basicProperties: null,
            body: body);
        Console.WriteLine(" [x] Published {0} to RabbitMQ", message);
        return true;
    }
}