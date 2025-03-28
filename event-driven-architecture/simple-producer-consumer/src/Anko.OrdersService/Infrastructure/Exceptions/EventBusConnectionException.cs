namespace Anko.OrdersService.Infrastructure.Exceptions;

public class EventBusConnectionException : Exception
{
    public string HostName { get; set; }

    public EventBusConnectionException(string hostName, string message) : base(message)
    {
        this.HostName = hostName;
    }
}