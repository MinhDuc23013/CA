using CA.Domain.Base;
using CA.Domain.Enums;
using CA.Domain.Events;
using CA.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Domain.Aggregates
{
    public sealed class LoanApplication : AggregateRoot
    {
        public string CIF { get; private set; } = string.Empty;
        public decimal RequestedAmount { get; private set; }
        public int TermInMonths { get; private set; }
        public LoanStatus Status { get; private set; }

        private LoanApplication() { } // For ORM

        private LoanApplication(string cif, decimal amount, int term)
        {
            Id = Guid.NewGuid();
            CIF = cif;
            RequestedAmount = amount;
            TermInMonths = term;
            Status = LoanStatus.Draft;

            AddDomainEvent(new LoanApplicationCreatedDomainEvent(Id, CIF));
        }

        public static LoanApplication Create(
            string CIF,
            decimal amount,
            int term)
        {
            if (amount < 0)
                throw new DomainException("Amount cannot be negative.");

            if (term <= 0)
                throw new DomainException("Loan term must be greater than zero.");

            return new LoanApplication(CIF, amount, term);
        }

        public void Submit()
        {
            if (Status != LoanStatus.Draft)
                throw new DomainException("Only draft loan can be submitted.");

            Status = LoanStatus.Submitted;
        }

        public void Approve()
        {
            if (Status != LoanStatus.Submitted)
                throw new DomainException("Only submitted loan can be approved.");

            Status = LoanStatus.Approved;
        }

        public void Reject()
        {
            if (Status != LoanStatus.Submitted)
                throw new DomainException("Only submitted loan can be rejected.");

            Status = LoanStatus.Rejected;
        }
    }

}
