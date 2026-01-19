using CA.Domain.LoanApplications.DomainEvents;
using CA.Domain.LoanApplications.Enums;
using CA.Domain.LoanApplications.Exceptions;
using CA.Domain.LoanApplications.SeedWord;

namespace CA.Domain.LoanApplications.Aggregates
{
    public sealed class LoanApplication : AggregateRoot<Guid>
    {
        public Guid CustomerId { get; private set; }
        public decimal RequestedAmount { get; private set; }
        public LoanStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private LoanApplication() { } // EF

        private LoanApplication(Guid id, Guid customerId, decimal requestedAmount)
        {
            Id = id;
            CustomerId = customerId;
            CustomerId = customerId;
            RequestedAmount = requestedAmount;
            Status = LoanStatus.Created;
            CreatedAt = DateTime.UtcNow;

            AddDomainEvent(new LoanApplicationCreatedDomainEvent(id, customerId));
        }

        public static LoanApplication Create(
            Guid customerId,
            decimal requestedAmount)
        {
            if (requestedAmount <= 0)
                throw new DomainException("Requested amount must be greater than zero");

            return new LoanApplication(
                Guid.NewGuid(),
                customerId,
                requestedAmount);
        }
    }


}
