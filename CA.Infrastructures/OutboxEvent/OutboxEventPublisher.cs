using CA.Application.Abstractions;
using CA.Application.Interfaces.DTOs;
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

        public async Task AddAsync(object integrationEvent, string queuename, string traceParentId, string traceState)
        {
            var message = new OutboxMessage
            {
                Type = integrationEvent.GetType().Name,
                Destination = queuename,
                Payload = JsonSerializer.Serialize(integrationEvent),
                OccurredAt = DateTime.UtcNow,
                Status = OutboxStatus.New.ToString(),
                TraceParent = traceParentId,
                TraceState = traceState
            };

            await _repo.AddAsync(message);
        }

        public async Task AddKafkaEvent(CreateLoanEventDTO loan)
        {
            var message = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "LoanApplicationCreated",
                Destination = "loan-applications",
                Payload = JsonSerializer.Serialize(new
                {
                    ApplicationId = loan.Id,
                    TermMonths = loan.TermMonths,
                    Amount = loan.Amount
                }),
                OccurredAt = DateTime.UtcNow,
                Status = loan.Status.ToString()
            };

            await _repo.AddAsync(message);

        }
    }

}
