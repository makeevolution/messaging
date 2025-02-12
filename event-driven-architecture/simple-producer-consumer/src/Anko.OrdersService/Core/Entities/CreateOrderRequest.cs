using System.Text.Json.Serialization;

namespace Anko.OrdersService.Core.Entities;

public class CreateOrderRequest
{
    [JsonPropertyName("customerId")]
    public string CustomerId { get; set; }
}