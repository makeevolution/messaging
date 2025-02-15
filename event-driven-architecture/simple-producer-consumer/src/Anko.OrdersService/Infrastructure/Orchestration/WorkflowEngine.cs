using Anko.OrdersService.Core.Entities;
using Anko.OrdersService.Core.Services;
using Anko.OrdersService.Infrastructure.Adapters.Database;
using Anko.OrdersService.Infrastructure.WorkflowEngine;
using Microsoft.EntityFrameworkCore;
using Temporalio.Client;
using Temporalio.Common;

namespace Anko.OrdersService.Infrastructure.Orchestration;

public class WorkflowEngine(ITemporalClient client, IServiceProvider serviceProvider) : IWorkflowEngine
{
    public async Task StartOrderWorkflowFor(string orderId)
    {
        using var scope = serviceProvider.CreateScope();
        var orderContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        var order2 =  await orderContext.Orders.FirstOrDefaultAsync(ordr => ordr.Id == orderId);
        var order = new Order();
        if (order is null)
        {
            throw new Exception($"Order {orderId} not found");
        };

        await client.StartWorkflowAsync(
            (OrderProcessingWorkflow wf) => wf.RunWorkflow(order),
            new WorkflowOptions(generateWorkflowIdFor(orderId), "orders-queue")
            {
                RetryPolicy = new RetryPolicy
                {
                    BackoffCoefficient = 2,
                    MaximumAttempts = 2,
                    NonRetryableErrorTypes = new List<string> { "Temporal.API.Error.AlreadyExists" }
                }
            });
    }
    private static string generateWorkflowIdFor(string orderId) => $"OrderProcessing_{orderId}";

}