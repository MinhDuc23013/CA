using CA.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Domain.Events
{
    public sealed class LoanApplicationCreatedDomainEvent : IDomainEvent
    {
        public Guid LoanApplicationId { get; }
        public string CIF { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public LoanApplicationCreatedDomainEvent(Guid loanApplicationId, string cif)
        {
            LoanApplicationId = loanApplicationId;
            CIF = cif;
        }
    }

}
