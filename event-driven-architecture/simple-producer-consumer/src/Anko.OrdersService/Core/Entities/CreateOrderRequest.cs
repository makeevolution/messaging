using System.Text.Json.Serialization;

namespace Anko.OrdersService.Core.Entities;

public class CreateOrderRequest
{
    [JsonPropertyName("Name")]
    public string OrderId { get; set; } 
    
}