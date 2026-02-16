using CA.Application.Abstractions;
using CA.Infrastructures.Persistence;
using CA.Infrastructures.Persistence.Interfaces;
using CA.Infrastructures.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CA.Infrastructures.OutboxEvent
{
    public class OutboxEventPublisher : IIntegrationEventPublisher
    {
        private readonly IOutboxRepository _repo;

        public OutboxEventPublisher(IOutboxRepository repo)
        {
            _repo = repo;
        }

        public async Task AddAsync(object integrationEvent, string queuename)
        {
            var message = new OutboxMessage
            {
                Type = integrationEvent.GetType().Name,
                Destination = queuename,
                Payload = JsonSerializer.Serialize(integrationEvent),
                OccurredAt = DateTime.UtcNow,
                Status = OutboxStatus.New.ToString()
            };

            await _repo.AddAsync(message);
        }
    }

}
