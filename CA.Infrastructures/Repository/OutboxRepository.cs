using CA.Application.Abstractions;
using CA.Infrastructures.Persistence;
using CA.Infrastructures.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CA.Infrastructures.Repository
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly AppDbContext _context;

        public OutboxRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OutboxMessage?> GetByIdAsync(Guid id)
        {
            return await _context.OutboxMessages.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(OutboxMessage outbox)
        {
            await _context.OutboxMessages.AddAsync(outbox);
        }
    }
}
