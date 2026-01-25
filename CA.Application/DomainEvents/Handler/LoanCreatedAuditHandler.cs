using CA.Application.Abstractions;
using CA.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.DomainEvents.Handler
{
    public sealed class LoanCreatedAuditHandler
        : IDomainEventHandler<LoanApplicationCreatedDomainEvent>
    {
        public Task HandleAsync(LoanApplicationCreatedDomainEvent domainEvent)
        {
            Console.WriteLine(
                $"[AUDIT] Loan {domainEvent.LoanApplicationId} created for customer {domainEvent.CIF}"
            );

            return Task.CompletedTask;
        }
    }

}
