using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.Abstractions
{
    public interface IIntegrationEventPublisher
    {
        Task AddAsync(object integrationEvent, string destination);
    }
}
