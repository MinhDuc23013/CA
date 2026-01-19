using CA.Application.Command;
using CA.Domain.LoanApplications.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.Interfaces
{
    public interface ILoanApplicationAppService
    {
        Task<Guid> CreateAsync(CreateLoanApplicationCommand command);
    }

}
