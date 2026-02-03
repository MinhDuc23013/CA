using CA.Application.Abstractions;
using CA.Application.Abstractions.DTO;
using CA.Infrastructures.EventBus.Kafka;
using CA.Infrastructures.EventBus.RabbiqMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Infrastructures.EventBus
{
    public sealed class MessageQueue : IMessageQueue
    {
        private readonly RabbitMqMessageQueue _rabbit;
        //private readonly KafkaMessageQueue _kafka;

        public MessageQueue(RabbitMqMessageQueue rabbit
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
