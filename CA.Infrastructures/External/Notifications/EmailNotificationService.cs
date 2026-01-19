using CA.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace CA.Infrastructures.External.Notifications
{
    public sealed class EmailNotificationService : INotificationService
    {
        private readonly SmtpClient _smtpClient;
        private readonly ICustomerRepository _customerRepository;
        private readonly EmailSettings _emailSettings;

        public EmailNotificationService(
            SmtpClient smtpClient,
            ICustomerRepository customerRepository,
            IOptions<EmailSettings> emailSettings)
        {
            _smtpClient = smtpClient;
            _customerRepository = customerRepository;
            _emailSettings = emailSettings.Value;
        }

        public async Task NotifyAsync(
            Guid customerId,
            string message,
            CancellationToken cancellationToken)
        {
            // 1️⃣ Lấy email user (Infrastructure có quyền query read-model)
            var customer = await _customerRepository
                .GetByIdAsync(customerId, cancellationToken);

            if (customer == null || string.IsNullOrWhiteSpace(customer.Email))
                return; // fail silent hoặc log

            // 2️⃣ Tạo mail
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.From),
                Subject = "Loan Application Notification",
                Body = message,
                IsBodyHtml = false
            };

            mailMessage.To.Add(customer.Email);

            // 3️⃣ Send email (SMTP)
            // SmtpClient không support CancellationToken → workaround bằng Task.Run
            await Task.Run(
                () => _smtpClient.Send(mailMessage),
                cancellationToken);
        }
    }
}