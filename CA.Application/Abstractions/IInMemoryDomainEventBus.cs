using CA.Domain.Base;

namespace CA.Application.Abstractions
{
    public interface IInMemoryDomainEventBus
    {
        Task PublishAsync(IEnumerable<IDomainEvent> domainEvents);
    }
}
