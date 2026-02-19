using CA.Application.Abstractions;
using CA.Application.Abstractions.DTO;
using CA.Application.Interfaces;
using CA.Application.Interfaces.DTOs;
using CA.Domain.Aggregates;
using CA.Domain.IRepositories;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Text.Json;

namespace CA.Application.Services
{

    public static class Telemetry
    {
        public static readonly ActivitySource Source =
            new("loan-service");
    }

    public sealed class LoanApplicationService : ILoanApplicationService
    {
        private readonly ILoanApplicationRepository _repository;
        private readonly IIntegrationEventPublisher _eventPublisher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApiIdempotencyRepository _idempotencyRepo;
        private readonly ILogger<LoanApplicationService> _logger;
        //public static readonly ActivitySource ActivitySource =
        //    new("loan-service");

        public LoanApplicationService(
            ILoanApplicationRepository repository,
            IUnitOfWork unitOfWork,
            IApiIdempotencyRepository idempotencyRepo,
            ILogger<LoanApplicationService> logger,
            IIntegrationEventPublisher eventPublisher)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
            _idempotencyRepo = idempotencyRepo;
            _logger = logger;

        }

        public async Task<CreateLoanApplicationResponse> CreateAsync(CreateLoanApplicationRequest request)
        {

            using var activity = Telemetry.Source.StartActivity("loan.create");

            try
            {

                activity?.SetTag("loan.cif", request.CIF);
                activity?.SetTag("loan.amount", request.Amount);
                activity?.SetTag("loan.term_months", request.TermMonths);
                activity?.SetTag("idempotency.key", request.IdempotencyKey);

                _logger.LogInformation("Start creating loan for CIF {CIF}", request.CIF);

                var requestJson = JsonSerializer.Serialize(request);
                var requestHash = _idempotencyRepo.HashRequest(requestJson);

                await _unitOfWork.BeginTransactionAsync();

                using (Telemetry.Source.StartActivity("idempotency.check"))
                {
                    var (isFirstRequest, record) =
                        await _idempotencyRepo.TryStartRequestAsync(request.IdempotencyKey, requestHash);

                    if (!isFirstRequest)
                    {
                        if (record!.RequestHash != requestHash)
                            throw new Exception("Idempotency key reused with different payload");

                        if (record.Status == "SUCCESS")
                        {
                            var cachedResponse =
                                JsonSerializer.Deserialize<CreateLoanApplicationResponse>(record.ResponseBody!);

                            if (cachedResponse is null)
                                throw new Exception("Cached response could not be deserialized");

                            _logger.LogInformation("Return cached response for key {Key}", request.IdempotencyKey);
                            return cachedResponse;
                        }

                        if (record.Status == "PROCESSING")
                            throw new Exception("Request is processing");

                        throw new Exception("Previous request failed");
                    }
                }

                LoanApplication loan;
                using (Telemetry.Source.StartActivity("domain.create-loan"))
                {
                    loan = LoanApplication.Create(
                        request.CIF,
                        request.Amount,
                        request.TermMonths,
                        request.IdempotencyKey);
                }

                activity?.SetTag("loan.id", loan.Id);


                using (Telemetry.Source.StartActivity("db.save-loan"))
                {
                    await _repository.AddAsync(loan);
                }

                _logger.LogInformation("Loan {LoanId} created", loan.Id);

                using (Telemetry.Source.StartActivity("integration.email-service"))
                {
                    await _eventPublisher.AddAsync(
                        new SendOrderEmailCommandRequest
                        {
                            To = "aaa@gmail.com",
                            Template = "3211",
                            Email = "ccc@makl.com",
                            Body = $"Your loan application with ID {loan.Id} has been created."
                        },
                        "email-service");
                }

                using (Telemetry.Source.StartActivity("db.commit-transaction"))
                {
                    await _unitOfWork.CommitAsync();
                }

                var response = new CreateLoanApplicationResponse
                {
                    LoanApplicationId = loan.Id,
                    Status = loan.Status.ToString()
                };

                var responseJson = JsonSerializer.Serialize(response);

                using (Telemetry.Source.StartActivity("idempotency.complete"))
                {
                    await _idempotencyRepo.CompleteRequestAsync(
                        request.IdempotencyKey,
                        responseJson,
                        200);
                }

                _logger.LogInformation("Loan {LoanId} completed successfully", loan.Id);

                return response;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                activity?.RecordException(ex);

                _logger.LogError(ex,
                    "Create loan failed for CIF {CIF} with key {Key}",
                    request.CIF,
                    request.IdempotencyKey);

                using (Telemetry.Source.StartActivity("idempotency.fail"))
                {
                    await _idempotencyRepo.FailRequestAsync(request.IdempotencyKey);
                }

                using (Telemetry.Source.StartActivity("db.rollback"))
                {
                    await _unitOfWork.RollbackAsync();
                }
                throw;
            }
        }
    }

}
