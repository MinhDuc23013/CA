using CA.Domain.LoanApplications.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Domain.LoanApplications.DomainEvents
{
    public sealed record LoanApplicationCreatedDomainEvent(Guid LoanApplicationId, Guid CustomerId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}
