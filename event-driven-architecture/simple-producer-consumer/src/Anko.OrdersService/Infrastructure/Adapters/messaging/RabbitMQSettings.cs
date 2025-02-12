namespace Anko.OrdersService.Infrastructure.Adapters.Messaging;

public class RabbitMqSettings
{
    public string HostName { get; set; } = "localhost";

    public string ExchangeName { get; set; } = "";
}