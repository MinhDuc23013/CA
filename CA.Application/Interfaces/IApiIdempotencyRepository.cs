using CA.Application.Interfaces.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.Interfaces
{
    public interface IApiIdempotencyRepository
    {
        Task<(bool IsFirstRequest, ApiIdempotencyDto? Record)> TryStartRequestAsync(Guid key, string requestHash);
        Task CompleteRequestAsync(Guid key, string responseJson, int statusCode);

        Task FailRequestAsync(Guid key);

        string HashRequest(string body);

    }
}
