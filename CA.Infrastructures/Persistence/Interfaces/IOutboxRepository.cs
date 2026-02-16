using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Infrastructures.Persistence.Interfaces
{
    public interface IOutboxRepository
    {
        Task<OutboxMessage?> GetByIdAsync(Guid id);
        Task AddAsync(OutboxMessage outboxMessage);
    }
}
