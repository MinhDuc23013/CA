using CA.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.Abstractions
{
    public interface IMessageQueue
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : class;

        Task SubscribeAsync<TEvent>(
            string subscriptionName,
            Func<TEvent, CancellationToken, Task> handler,
            CancellationToken cancellationToken = default)
            where TEvent : class;
    }


}
