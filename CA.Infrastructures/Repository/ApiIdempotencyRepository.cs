using CA.Application.Common;
using CA.Application.Interfaces;
using CA.Application.Interfaces.DTOs;
using CA.Infrastructures.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CA.Infrastructures.Repository
{
    public class ApiIdempotencyRepository : IApiIdempotencyRepository
    {
        private readonly AppDbContext _context;

        public ApiIdempotencyRepository(AppDbContext context)
        {
            _context = context;
        }
        public string HashRequest(string body)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(body));
            return Convert.ToHexString(bytes);
        }

        public async Task<(bool IsFirstRequest, ApiIdempotencyDto? Record)>
            TryStartRequestAsync(Guid key, string requestHash)
        {
            var entity = new ApiIdempotency
            {
                IdempotencyKey = key,
                RequestHash = requestHash,
                Status = IdempotencyStatus.PROCESSING.ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            _context.ApiIdempotencies.Add(entity);

            try
            {
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                _context.Entry(entity).State = EntityState.Detached;
                var existing = await _context.ApiIdempotencies
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdempotencyKey == key);

                if (existing != null)
                {
                    return (false, new ApiIdempotencyDto
                    {
                        IdempotencyKey = existing.IdempotencyKey,
                        RequestHash = existing.RequestHash,
                        Status = existing.Status,
                        ResponseBody = existing.ResponseBody,
                        HttpStatus = existing.HttpStatus,
                        CreatedAt = existing.CreatedAt,
                        ExpiresAt = existing.ExpiresAt
                    });
                }

                return (false, null);
            }
        }

        public async Task CompleteRequestAsync(Guid key, string responseJson, int statusCode)
        {
            var entity = await _context.ApiIdempotencies
                .FirstAsync(x => x.IdempotencyKey == key);

            entity.Status = IdempotencyStatus.SUCCESS.ToString();
            entity.ResponseBody = responseJson;
            entity.HttpStatus = statusCode;

            await _context.SaveChangesAsync();
        }

        public async Task FailRequestAsync(Guid key)
        {
            var entity = await _context.ApiIdempotencies
                .FirstAsync(x => x.IdempotencyKey == key);

            entity.Status = IdempotencyStatus.FAILED.ToString();

            await _context.SaveChangesAsync();
        }

    }
}
