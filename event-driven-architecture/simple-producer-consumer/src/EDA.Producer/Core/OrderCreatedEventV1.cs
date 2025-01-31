using EDA.Events;

namespace EDA.Producer.Core;

public class OrderCreatedEventV1 : BaseEvent
{
    public string OrderId { get; set; }
    public override string EventName => "order.orderCreated";  // This is the routing key consumer will listen to
    public override string EventVersion => "v1";
    public override Uri Source => new("https://orders.EDA");
}