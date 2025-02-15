using Temporalio.Activities;

namespace Anko.OrdersService.Infrastructure.WorkflowEngine;

/* Define all possible steps i.e. Activities for the order service, that can be used inside a workflow */
public class OrderSteps
{
    // [Activity]
    // public async Task SubmitOrder(OrderDto order)
    // {
    //     await submitOrderHandler.Handle(new SubmitOrderCommand()
    //     {
    //         OrderIdentifier = order.OrderIdentifier,
    //         CustomerIdentifier = order.CustomerIdentifier,
    //     });
    // }
}