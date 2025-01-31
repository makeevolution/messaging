using EDA.Events;

namespace EDA.Consumer.Core.ExternalEvents;

public class OrderCreatedEventV1 : BaseEvent
{
    public string OrderId { get; set; }
    public override string EventName => "order.orderCompleted";
    public override string EventVersion => "v1";
    public override Uri Source => new("https://orders.EDA");
}