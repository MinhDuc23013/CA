using CA.Application.Abstractions;
using CA.Application.Abstractions.DTO;
using CA.Application.Interfaces;
using CA.Application.Interfaces.DTOs;
using CA.Domain.Aggregates;
using CA.Domain.IRepositories;
using System.Text.Json;

namespace CA.Application.Services
{
    public sealed class LoanApplicationService : ILoanApplicationService
    {
        private readonly ILoanApplicationRepository _repository;
        private readonly IIntegrationEventPublisher _eventPublisher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApiIdempotencyRepository _idempotencyRepo;
        //private readonly IMessageQueue _messageQueue;

        public LoanApplicationService(
            ILoanApplicationRepository repository,
            IUnitOfWork unitOfWork,
            IApiIdempotencyRepository idempotencyRepo,
            IIntegrationEventPublisher eventPublisher)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
            _idempotencyRepo = idempotencyRepo;

        }

        public async Task<CreateLoanApplicationResponse> CreateAsync(CreateLoanApplicationRequest request)
        {

            var requestJson = JsonSerializer.Serialize(request);
            var requestHash = _idempotencyRepo.HashRequest(requestJson);

            await _unitOfWork.BeginTransactionAsync();

            var (isFirstRequest, record) = await _idempotencyRepo.TryStartRequestAsync(request.IdempotencyKey, requestHash);

            if (!isFirstRequest)
            {
                if (record!.RequestHash != requestHash)
                    throw new("Idempotency key reused with different payload");

                if (record.Status == "SUCCESS")
                {
                    var response = JsonSerializer.Deserialize<CreateLoanApplicationResponse>(record.ResponseBody!);
                    if (response is null)
                        throw new("Idempotency record response body could not be deserialized");
                    return response;
                }

                if (record.Status == "PROCESSING")
                    throw new("Request is processing");

                throw new("Previous request failed");
            }

            try
            {
                var loan = LoanApplication.Create(
                    request.CIF,
                    request.Amount,
                    request.TermMonths,
                    request.IdempotencyKey);

                await _repository.AddAsync(loan);

                await _eventPublisher.AddAsync(
                    new SendOrderEmailCommandRequest
                    {
                        To = "aaa@gmail.com",
                        Template = "3211",
                        Email = "ccc@makl.com",
                        Body = $"Your loan application with ID {loan.Id} has been created."
                    },
                    "email-service"
                );

                await _unitOfWork.CommitAsync();

                var response = new CreateLoanApplicationResponse
                {
                    LoanApplicationId = loan.Id,
                    Status = loan.Status.ToString()
                };

                var responseJson = JsonSerializer.Serialize(response);

                await _idempotencyRepo.CompleteRequestAsync(
                    request.IdempotencyKey,
                    responseJson,
                    200);

                await _unitOfWork.BeginTransactionAsync();

                return response;
            }
            catch
            {
                await _idempotencyRepo.FailRequestAsync(request.IdempotencyKey);
                await _unitOfWork.RollbackAsync();
                throw;
            }



            //var loan = LoanApplication.Create(request.CIF, request.Amount, request.TermMonths, request.IdempotencyKey);

            //await _repository.AddAsync(loan);

            //await _eventPublisher.AddAsync(
            //    new SendOrderEmailCommandRequest
            //    {
            //        To = "aaa@gmail.com",
            //        Template = "3211",
            //        Email = "ccc@makl.com",
            //        Body = $"Your loan application with ID {loan.Id} has been created."
            //    },
            //    "email-service"
            //);


            //await _unitOfWork.CommitAsync();

            //return new CreateLoanApplicationResponse()
            //{
            //    LoanApplicationId = loan.Id,
            //    Status = loan.Status.ToString()
            //};
        }
    }

}
