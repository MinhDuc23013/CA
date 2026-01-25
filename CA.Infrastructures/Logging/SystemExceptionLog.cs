using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Infrastructures.Logging
{
    public sealed class SystemExceptionLog
    {
        public long Id { get; private set; }

        public string ApplicationName { get; private set; } = default!;

        public string? ServiceName { get; private set; }

        public string Message { get; private set; } = default!;

        public string? RequestPath { get; private set; }

        public string? HttpMethod { get; private set; }

        public int? StatusCode { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private SystemExceptionLog() { } // For ORM

        public SystemExceptionLog(
            string applicationName,
            string message,
            string? serviceName = null,
            string? requestPath = null,
            string? httpMethod = null,
            int? statusCode = null)
        {
            ApplicationName = applicationName;
            Message = message;
            ServiceName = serviceName;
            RequestPath = requestPath;
            HttpMethod = httpMethod;
            StatusCode = statusCode;
            CreatedAt = DateTime.UtcNow;
        }
    }

}
