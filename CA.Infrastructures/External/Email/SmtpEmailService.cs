using CA.Application.Abstractions;

namespace CA.Infrastructures.External.Email
{
    public sealed class SmtpEmailService : IEmailService
    {
        public Task SendAsync(string to, string subject, string body)
        {
            // SMTP / SendGrid / SES
            return Task.CompletedTask;
        }
    }

}
