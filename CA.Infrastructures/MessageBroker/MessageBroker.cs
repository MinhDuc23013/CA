using CA.Application.Abstractions;
using CA.Application.Abstractions.DTO;
using CA.Infrastructures.MessageBroker.RabbiqMq;

namespace CA.Infrastructures.EventBus
{
    public sealed class MessageBroker : IMessageQueue
    {
        private readonly RabbitMqMessageQueue _rabbit;
        //private readonly KafkaMessageQueue _kafka;

        public MessageBroker(RabbitMqMessageQueue rabbit
            //KafkaMessageQueue kafka
            )
        {
            _rabbit = rabbit;
            //_kafka = kafka;
        }

        public Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class
        {
            if (message is SendOrderEmailCommandRequest)
                return _rabbit.PublishAsync(topic, message);
            return Task.CompletedTask;

            //return _kafka.PublishAsync(topic, message);
        }
    }
}
