using CA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.Interfaces.DTOs
{
    public class CreateLoanEventDTO
    {
        public Guid Id { get; set; }
        public required string CIF { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public int TermMonths { get; set; }
        public Guid IdempotencyKey { get; set; }
        public LoanStatus Status { get;  set; }
    }
}
