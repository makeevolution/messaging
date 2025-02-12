using Anko.PaymentsService.Core.ExternalEvents;

namespace Anko.PaymentsService.Core;

/* Consumer/handler of orderSubmitted events 
The handler is rigged to throw error if the orderId starts with 6, 
to demonstrate that errored messages are thrown into the dead letter queue
*/


public class orderSubmittedEventHandler(ILogger<orderSubmittedEventHandler> logger)
{
    public async Task Handle(OrderSubmittedEventV1 orderSubmittedEventV1)
    {
        logger.LogInformation($"Order {orderSubmittedEventV1.OrderId} has been submitted");
        logger.LogInformation("Order {orderId} will be processed shortly", orderSubmittedEventV1.OrderId);

        if (orderSubmittedEventV1.OrderId.StartsWith("6"))
        {
            throw new Exception("Invalid event");
        }
    }    
}