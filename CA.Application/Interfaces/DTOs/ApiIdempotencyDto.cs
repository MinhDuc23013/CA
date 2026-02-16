using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.Interfaces.DTOs
{
    public class ApiIdempotencyDto
    {
        public Guid IdempotencyKey { get; set; }

        public string RequestHash { get; set; } = null!;

        public string Status { get; set; }

        public string? ResponseBody { get; set; }

        public int? HttpStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }
}
