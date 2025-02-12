using Anko.OrdersService.Infrastructure.WorkflowEngine;
using Temporalio.Extensions.Hosting;

namespace Anko.OrdersService.Workers;

public static class TemporalConfiguration
{
    public static IServiceCollection AddTemporalWorkflows(this IServiceCollection services,
        IConfiguration configuration)
    {
        var temporalEndpoint = configuration["TEMPORAL_ENDPOINT"];

        if (string.IsNullOrEmpty(temporalEndpoint)) return services;

        services.AddHostedTemporalWorker("orders-queue")
            .AddSingletonActivities<OrderSteps>()
            .AddWorkflow<OrderProcessingWorkflow>();

        return services;
    }
}