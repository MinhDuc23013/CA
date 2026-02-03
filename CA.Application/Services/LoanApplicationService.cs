using CA.Application.Abstractions;
using CA.Application.Abstractions.DTO;
using CA.Application.Interfaces;
using CA.Application.Interfaces.DTOs;
using CA.Domain.Aggregates;
using CA.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EventBus;

namespace CA.Application.Services
{
    public sealed class LoanApplicationService : ILoanApplicationService
    {
        private readonly ILoanApplicationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageQueue _messageQueue;

        public LoanApplicationService(
            ILoanApplicationRepository repository,
            IUnitOfWork unitOfWork,
            IMessageQueue messageQueue)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _messageQueue = messageQueue;
        }

        public async Task<CreateLoanApplicationResponse> CreateAsync(CreateLoanApplicationRequest request)
        {
            var loan = LoanApplication.Create(request.CIF, request.Amount, request.TermMonths);

            await _repository.AddAsync(loan);
            await _unitOfWork.CommitAsync();

            await _messageQueue.PublishAsync("email-service", new SendOrderEmailCommandRequest
            {
                To = "aaa@gmail.com",
                Template = "3211",
                Email= "ccc@makl.com",
                Body = $"Your loan application with ID {loan.Id} has been created."
            });

            return new CreateLoanApplicationResponse()
            {
                LoanApplicationId = loan.Id,
                Status = loan.Status.ToString()
            };
        }
    }

}
