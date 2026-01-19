using CA.Domain.LoanApplications.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Infrastructures.Dispatchers
{
    public interface IDomainEventDispatcher
    {
        Task Dispatch(IDomainEvent domainEvent);
    }

}
