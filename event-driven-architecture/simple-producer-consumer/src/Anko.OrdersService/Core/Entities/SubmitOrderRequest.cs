using System.Text.Json.Serialization;

namespace Anko.OrdersService.Core.Entities;

public class SubmitOrderRequest
{
    [JsonPropertyName("OrderId")]
    public string OrderId { get; set; }
}