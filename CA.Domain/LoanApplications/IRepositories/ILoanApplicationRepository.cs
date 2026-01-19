using CA.Domain.LoanApplications.Aggregates;

namespace CA.Domain.LoanApplications.IRepositories
{
    public interface ILoanApplicationRepository
    {
        Task AddAsync(LoanApplication loanApplication);
        Task<LoanApplication?> GetByIdAsync(Guid id);
    }
}
