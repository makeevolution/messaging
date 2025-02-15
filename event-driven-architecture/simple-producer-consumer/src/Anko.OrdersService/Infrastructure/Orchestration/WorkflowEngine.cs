using Anko.OrdersService.Core.Services;
using Anko.OrdersService.Infrastructure.WorkflowEngine;
using Temporalio.Client;

namespace Anko.OrdersService.Infrastructure.Orchestration;

public class WorkflowEngine(ITemporalClient client, IOrderRepository orderRepository) : IWorkflowEngine
{
    public async Task StartOrderWorkflowFor(string orderIdentifier)
    {
        var order = await orderRepository.Retrieve(orderIdentifier);

        if (order is null) return;

        await client.StartWorkflowAsync(
            (OrderProcessingWorkflow wf) => wf.RunAsync(new OrderDto(order)),
            new WorkflowOptions(generateWorkflowIdFor(orderIdentifier), "orders-queue")
            {
                RetryPolicy = new RetryPolicy
                {
                    BackoffCoefficient = 2,
                    MaximumAttempts = 2,
                    NonRetryableErrorTypes = new List<string> { "Temporal.API.Error.AlreadyExists" }
                }
            });
    }
    private static string generateWorkflowIdFor(string orderIdentifier) => $"OrderProcessing_{orderIdentifier}";

}