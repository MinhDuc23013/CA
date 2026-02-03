using CA.Application.Abstractions;
using CA.Application.Abstractions.DTO;
using CA.Infrastructures.EventBus.RabbiqMq.DTO;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CA.Infrastructures.EventBus.Kafka
{
    public sealed class KafkaMessageQueue : IDisposable
    {
        private readonly IProducer<string, string> _producer;

        public KafkaMessageQueue(IOptions<KafkaOptions> options)
        {
            var value = options.Value;
            var config = new ProducerConfig
            {
                BootstrapServers = value.BootstrapServers,
                ClientId = value.ClientId,
                Acks = Acks.All,
                EnableIdempotence = true, // tránh duplicate
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 100
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublishAsync<T>(
            string topic,
            T message,
            CancellationToken cancellationToken = default)
            where T : class
        {
            var payload = JsonSerializer.Serialize(message);

            var kafkaMessage = new Message<string, string>
            {
                Key = typeof(T).Name, // partition key
                Value = payload
            };

            try
            {
                await _producer.ProduceAsync(
                    topic,
                    kafkaMessage,
                    cancellationToken);
            }
            catch (ProduceException<string, string> ex)
            {
                // log ở đây
                throw;
            }
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }

    }
}
