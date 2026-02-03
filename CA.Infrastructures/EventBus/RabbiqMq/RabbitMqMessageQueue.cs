using CA.Application.Abstractions;
using CA.Infrastructures.EventBus.RabbiqMq.DTO;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace CA.Infrastructures.EventBus.RabbiqMq
{
    public sealed class RabbitMqMessageQueue : IDisposable
    {
        private readonly IConnection _connection;
        private readonly RabbitMQ.Client.IModel _channel;

        public RabbitMqMessageQueue(IOptions<RabbitMqOptions> options)
        {
            var opt = options.Value;
            var factory = new ConnectionFactory
            {
                HostName = options.Value.HostName,
                UserName = options.Value.UserName,
                Password = options.Value.Password,
                Port = options.Value.Port,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

        }
        public Task PublishAsync<T>(
            string topic,
            T message,
            CancellationToken cancellationToken = default)
            where T : class
        {
            _channel.BasicPublish("", topic, null,
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}

