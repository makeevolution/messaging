using Anko.Events;

namespace Anko.PaymentsService.Core.ExternalEvents;

public class OrderSubmittedEventV1 : BaseEvent
{
    public string OrderId { get; set; }
    public override string EventName => "order.orderSubmitted";
    public override string EventVersion => "v1";
    public override Uri Source => new("https://orders.EDA");
}