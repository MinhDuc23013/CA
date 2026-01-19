namespace CA.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyAsync( Guid customerId, string message);
    }

}
