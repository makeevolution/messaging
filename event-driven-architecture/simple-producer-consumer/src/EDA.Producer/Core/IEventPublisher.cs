namespace EDA.Producer.Core;

public interface IEventPublisher
{
    Task Publish(OrderCreatedEventV1 evt);
}