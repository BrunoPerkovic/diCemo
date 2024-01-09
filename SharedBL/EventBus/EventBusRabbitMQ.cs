/*using System.Text;
using RabbitMQ.Client;

namespace SharedBL.EventBus;

public class EventBusRabbitMQ : IEventBus, IDisposable
{
    private readonly string _connectionString;
    private readonly string _brokerName;

    public EventBusRabbitMQ(string connectionString, string brokerName)
    {
        _connectionString = connectionString;
        _brokerName = brokerName;
    }

    public void Publish(IntegrationEvent @event)
    {
        var eventName = @event.GetType()
            .Name;
        var factory = new ConnectionFactory() { HostName = _connectionString };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        /*
        channel.QueueDeclare(queue: eventName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);#1#
        {
            channel.ExchangeDeclare(exchange: _brokerName,
                type: "direct");
            string message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: _brokerName,
                routingKey: eventName,
                basicProperties: null,
                body: body);
        }
    }
}*/