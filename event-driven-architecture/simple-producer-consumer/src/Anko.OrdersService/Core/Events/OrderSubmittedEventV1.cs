using Anko.Events;

namespace Anko.OrdersService.Core.Events;

public class orderSubmittedEventV1 : BaseEvent
{
    public string OrderId { get; set; }
    public override string EventName => "order.orderSubmitted";  // This is the routing key consumer will listen to
    public override string EventVersion => "v1";
    public override Uri Source => new("https://orders.EDA");
}