using CA.Domain.LoanApplications.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.Abstracts
{
    public interface IDomainEventHandler<in TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        Task Handle(TDomainEvent domainEvent);
    }

}
