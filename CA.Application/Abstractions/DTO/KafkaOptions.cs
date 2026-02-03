using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.Abstractions.DTO
{
    public sealed class KafkaOptions
    {
        public string BootstrapServers { get; init; } = default!;
        public string ClientId { get; init; }
    }
}
