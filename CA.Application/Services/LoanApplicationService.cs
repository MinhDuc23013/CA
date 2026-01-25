using CA.Application.Abstractions;
using CA.Application.Interfaces;
using CA.Application.Interfaces.DTOs;
using CA.Domain.Aggregates;
using CA.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.Services
{
    public sealed class LoanApplicationService : ILoanApplicationService
    {
        private readonly ILoanApplicationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public LoanApplicationService(
            ILoanApplicationRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateLoanApplicationResponse> CreateAsync(CreateLoanApplicationRequest request)
        {
            var loan = LoanApplication.Create(request.CIF, request.Amount, request.TermMonths);

            await _repository.AddAsync(loan);
            await _unitOfWork.CommitAsync();

            return new CreateLoanApplicationResponse()
            {

            };
        }
    }

}
