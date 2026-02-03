using CA.Application.Abstractions;
using CA.Infrastructures.EventBus;

namespace CA.Infrastructures.External.Email
{
    public class SmtpEmailService : IEmailService
    {
        public async Task SendAsync(string to, string subject, string body)
        {
        }
    }

}
