namespace Anko.OrdersService.Adapters;

public class RabbitMqSettings
{
    public string HostName { get; set; } = "localhost";

    public string ExchangeName { get; set; } = "";
}