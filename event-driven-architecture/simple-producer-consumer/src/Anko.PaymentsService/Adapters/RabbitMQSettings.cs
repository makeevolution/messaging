namespace Anko.PaymentsService.Adapters;

public class RabbitMqSettings
{
    public string HostName { get; set; } = "localhost";

    public string ExchangeName { get; set; } = "";
    public string DeadLetterExchangeName { get; set; } = "";
}