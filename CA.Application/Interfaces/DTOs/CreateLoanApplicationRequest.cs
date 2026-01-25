using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.Interfaces.DTOs
{
    public sealed class CreateLoanApplicationRequest
    {
        public required string CIF { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public int TermMonths { get; set; }
    }
}
