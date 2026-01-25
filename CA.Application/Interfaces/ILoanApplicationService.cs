using CA.Application.Interfaces.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.Interfaces
{
    public interface ILoanApplicationService
    {
        Task<CreateLoanApplicationResponse> CreateAsync(CreateLoanApplicationRequest request);
    }
}
