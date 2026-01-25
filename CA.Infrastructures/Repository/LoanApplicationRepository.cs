using CA.Domain.Aggregates;
using CA.Domain.IRepositories;
using CA.Infrastructures.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CA.Infrastructures.Repository
{
    public sealed class LoanApplicationRepository
        : ILoanApplicationRepository
    {
        private readonly AppDbContext _context;

        public LoanApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LoanApplication?> GetByIdAsync(Guid id)
        {
            return await _context.LoanApplications
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(LoanApplication loanApplication)
        {
            await _context.LoanApplications.AddAsync(loanApplication);
        }
    }


}
