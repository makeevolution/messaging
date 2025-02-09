namespace Anko.OrdersService.Core;

public interface IEventPublisher
{
    Task Publish(OrderCreatedEventV1 evt);
}