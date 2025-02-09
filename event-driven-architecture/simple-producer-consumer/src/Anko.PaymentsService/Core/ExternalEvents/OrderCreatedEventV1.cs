using Anko.Events;

namespace Anko.PaymentsService.Core.ExternalEvents;

public class OrderCreatedEventV1 : BaseEvent
{
    public string OrderId { get; set; }
    public override string EventName => "order.orderCreated";
    public override string EventVersion => "v1";
    public override Uri Source => new("https://orders.EDA");
}