using CA.Application.Abstractions;
using CA.Domain.Base;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Infrastructures.EventBus
{
    public sealed class InMemoryDomainEventBus : IInMemoryDomainEventBus
    {
        private readonly IServiceProvider _serviceProvider;

        public InMemoryDomainEventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                var handlerType = typeof(IDomainEventHandler<>)
                    .MakeGenericType(domainEvent.GetType());

                var handlers = _serviceProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    var method = handlerType.GetMethod("HandleAsync");
                    if (method == null) continue;

                    await (Task)method.Invoke(handler, new[] { domainEvent })!;
                }
            }
        }
    }

}
