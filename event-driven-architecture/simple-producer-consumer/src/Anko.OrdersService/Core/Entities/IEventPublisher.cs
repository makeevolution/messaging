using Anko.OrdersService.Core.Events;

namespace Anko.OrdersService.Core.Entities;

public interface IEventPublisher
{
    Task Publish(OrderSubmittedEventV1 evt);
}