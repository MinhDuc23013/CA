using CA.Application.Abstracts;
using CA.Domain.LoanApplications.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.EventHandler
{
    public class LoanApplicationCreatedHandler
        : IDomainEventHandler<LoanApplicationCreatedDomainEvent>
    {
        private readonly INotificationService _notificationService;

        public LoanApplicationCreatedHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(LoanApplicationCreatedDomainEvent notification)
        {
            await _notificationService.NotifyAsync(
                notification.CustomerId,
                "Your loan application has been created successfully");
        }
    }

}
