using CA.Domain.LoanApplications.Abstractions;
using CA.Infrastructures.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Infrastructures.Implementation
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Dispatch(IDomainEvent domainEvent)
        {
            var handlerType = typeof(IDomainEventHandler)
                .MakeGenericType(domainEvent.GetType());

            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (dynamic handler in handlers)
            {
                await handler.Handle((dynamic)domainEvent);
            }
        }
    }

}
