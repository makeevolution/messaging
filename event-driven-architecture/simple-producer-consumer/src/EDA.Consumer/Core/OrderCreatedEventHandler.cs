using EDA.Consumer.Core.ExternalEvents;
using EDA.Events;

namespace EDA.Consumer.Core;

/* Consumer/handler of OrderCreated events 
The handler is rigged to throw error if the orderId starts with 6, 
to demonstrate that errored messages are thrown into the dead letter queue
*/
public class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
{
    public async Task Handle(OrderCreatedEventV1 orderCreatedEventV1)
    {

        logger.LogInformation($"Order {orderCreatedEventV1.OrderId} has been created");

        if (orderCreatedEventV1.OrderId.StartsWith("6"))
        {
            throw new Exception("Invalid event");
        }
    }    
}