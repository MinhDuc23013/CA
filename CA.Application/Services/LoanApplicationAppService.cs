using CA.Application.Command;
using CA.Application.Interfaces;
using CA.Domain.LoanApplications.Aggregates;
using CA.Domain.LoanApplications.IRepositories;
using Volo.Abp;
using Volo.Abp.Uow;

namespace CA.Application.Services
{
    public class LoanApplicationAppService : ILoanApplicationAppService
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LoanApplicationAppService(
            ILoanApplicationRepository loanApplicationRepository,
            IUnitOfWork unitOfWork)
        {
            _loanApplicationRepository = loanApplicationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateAsync( CreateLoanApplicationCommand command)
        {

            var loanExits = await _loanApplicationRepository
                .GetByIdAsync(command.CustomerId);

            if (loanExits != null)
                throw new BusinessException(
                    "Customer already has a loan application");

            var loanApplication = LoanApplication.Create(
                command.CustomerId,
                command.RequestedAmount);

            await _loanApplicationRepository.AddAsync(loanApplication);

            await _unitOfWork.SaveChangesAsync();

            return loanApplication.Id;
        }
    }

}
