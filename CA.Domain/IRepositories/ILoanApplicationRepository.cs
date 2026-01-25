using CA.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Domain.IRepositories
{
    public interface ILoanApplicationRepository
    {
        Task<LoanApplication?> GetByIdAsync(Guid id);
        Task AddAsync(LoanApplication loanApplication);
    }

}
