using CA.Domain.LoanApplications.Aggregates;
using CA.Domain.LoanApplications.IRepositories;
using CA.Infrastructures.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CA.Infrastructures.Repository
{
    public class LoanApplicationRepository : ILoanApplicationRepository
    {
        private readonly AppDbContext _context;

        public LoanApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LoanApplication?> GetByIdAsync( Guid id)
        {
            return await _context.LoanApplications
                // Load entity con trong Aggregate
                //.Include(x => x.Disbursements)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync( LoanApplication loan)
        {
            _context.LoanApplications.Add(loan);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync(
            LoanApplication loan,
            CancellationToken ct = default)
        {
            // EF Core tự track Aggregate + entity con
            _context.LoanApplications.Update(loan);
            await _context.SaveChangesAsync(ct);
        }
    }

}
