using CA.Application.Interfaces;
using CA.Infrastructures.Persistence;

namespace CA.Infrastructures.Implementation
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public EfUnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

}
