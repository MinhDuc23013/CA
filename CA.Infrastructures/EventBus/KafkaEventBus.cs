using CA.Application.Abstractions;
using Confluent.Kafka;
using System.Text.Json;

namespace CA.Infrastructures.EventBus
{
    public sealed class KafkaEventBus : IMessageQueue, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ConsumerConfig _consumerConfig;

        public KafkaEventBus(
            ProducerConfig producerConfig,
            ConsumerConfig consumerConfig)
        {
            _producer = new ProducerBuilder<string, string>(producerConfig)
                .Build();

            _consumerConfig = consumerConfig;
        }

        public async Task PublishAsync<TEvent>(
            TEvent @event,
            CancellationToken cancellationToken = default)
            where TEvent : class
        {
            var topic = typeof(TEvent).Name;

            var message = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event)
            };

            await _producer.ProduceAsync(topic, message, cancellationToken);
        }

        public Task SubscribeAsync<TEvent>(
            string subscriptionName,
            Func<TEvent, CancellationToken, Task> handler,
            CancellationToken cancellationToken = default)
            where TEvent : class
        {
            return Task.Run(async () =>
            {
                var config = new ConsumerConfig(_consumerConfig)
                {
                    GroupId = subscriptionName,
                    EnableAutoCommit = false,
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                using var consumer = new ConsumerBuilder<string, string>(config)
                    .SetValueDeserializer(Deserializers.Utf8)
                    .Build();

                var topic = typeof(TEvent).Name;
                consumer.Subscribe(topic);

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(cancellationToken);

                        var @event = JsonSerializer.Deserialize<TEvent>(
                            result.Message.Value)!;

                        await handler(@event, cancellationToken);

                        consumer.Commit(result);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        // LOG ERROR
                        // Không commit → message sẽ được retry
                    }
                }

                consumer.Close();
            }, cancellationToken);
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }

    }
}
