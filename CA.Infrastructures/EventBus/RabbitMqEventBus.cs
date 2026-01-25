using CA.Application.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CA.Infrastructures.EventBus
{
    public sealed class RabbitMqEventBus : IMessageQueue, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public RabbitMqEventBus(string connectionString)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : class
        {
            var exchange = typeof(TEvent).Name;

            _channel.ExchangeDeclareAsync(
                exchange: exchange,
                type: ExchangeType.Fanout,
                durable: true);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

            _channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: string.Empty,
                body: body);

            return Task.CompletedTask;
        }

        public Task SubscribeAsync<TEvent>(
            string subscriptionName,
            Func<TEvent, CancellationToken, Task> handler,
            CancellationToken cancellationToken = default)
            where TEvent : class
        {
            var exchange = typeof(TEvent).Name;

            _channel.ExchangeDeclareAsync(exchange, ExchangeType.Fanout, true);
            _channel.QueueDeclareAsync(subscriptionName, true, false, false);
            _channel.QueueBindAsync(subscriptionName, exchange, string.Empty);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, args) =>
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var @event = JsonSerializer.Deserialize<TEvent>(json)!;

                await handler(@event, cancellationToken);
                await _channel.BasicAckAsync(args.DeliveryTag, false);
            };

            _channel.BasicConsumeAsync(
                queue: subscriptionName,
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel.CloseAsync().GetAwaiter().GetResult();
            _connection.CloseAsync().GetAwaiter().GetResult();
        }
    }
}

