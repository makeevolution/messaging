using Anko.OrdersService.Core.Entities;
using Anko.OrdersService.Core.Services;
using Temporalio.Workflows;

namespace Anko.OrdersService.Infrastructure.WorkflowEngine;

[Workflow]
public class OrderProcessingWorkflow : IOrderWorkflow
{
    [WorkflowRun]
    public async Task<IResult> RunWorkflow(Order order)
    {
        Console.WriteLine("Order processing workflow started");
        return Results.Created($"OrderProcessingWorkflow", order);
    }

    public Task SubmitOrder()
    {
        throw new NotImplementedException();
    }
}