using EDA.Consumer.Core.ExternalEvents;

namespace EDA.Consumer.Core;

/* Consumer/handler of OrderCreated events 
The handler is rigged to throw error if the orderId starts with 6, 
to demonstrate that errored messages are thrown into the dead letter queue
*/
public class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
{
    public async Task Handle(OrderCreatedEvent orderCreatedEvent)
    {
        logger.LogInformation($"Order {orderCreatedEvent.OrderId} has been created");

        if (orderCreatedEvent.OrderId.StartsWith("6"))
        {
            throw new Exception("Invalid event");
        }
    }    
}