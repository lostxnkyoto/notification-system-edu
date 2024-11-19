using System.Text;
using Newtonsoft.Json;
using NotificationSystem.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationSystem.Brokers;

public class RabbitMqBroker : IMessageBroker
{
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqBroker> _logger;

    public RabbitMqBroker(IConfiguration configuration, ILogger<RabbitMqBroker> logger)
    {
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:Host"],
            Port = int.Parse(configuration["RabbitMq:Port"] ?? "5672"),
            UserName = configuration["RabbitMq:Username"],
            Password = configuration["RabbitMq:Password"]
        };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "notification_exchange", type: ExchangeType.Fanout);
        _logger.LogInformation("RabbitMQ connection established and exchange declared.");
    }

    public Task PublishAsync(NotificationEvent notificationEvent)
    {
        var message = JsonConvert.SerializeObject(notificationEvent);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "notification_exchange", routingKey: "", basicProperties: null, body: body);
        _logger.LogInformation($"Published message to RabbitMQ: {message}");
        return Task.CompletedTask;
    }

    public void Subscribe(string queueName, Func<NotificationEvent, Task> onMessageReceived)
    {
        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(queue: queueName, exchange: "notification_exchange", routingKey: "");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var notificationEvent = JsonConvert.DeserializeObject<NotificationEvent>(message);
            _logger.LogInformation($"Message received from RabbitMQ: {message}");
            await onMessageReceived(notificationEvent);
        };
        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }
}