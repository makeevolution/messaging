using System.Text.Json.Serialization;

namespace Anko.OrdersService.Core;

public class CreateOrderRequest
{
    [JsonPropertyName("customerId")]
    public string CustomerId { get; set; }
}