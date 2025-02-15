using Anko.OrdersService.Core.Services;
using Anko.OrdersService.Infrastructure.Orchestration;
using Anko.OrdersService.Infrastructure.WorkflowEngine;
using Temporalio.Client;
using Temporalio.Extensions.Hosting;
using Temporalio.Extensions.OpenTelemetry;

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

    public static IServiceCollection ConfigureTemporalEngine(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IWorkflowEngine, WorkflowEngine>();
        services.AddTemporalClient(options =>
        {
            options.TargetHost = configuration["TEMPORAL_ENDPOINT"];
            options.Tls = (configuration["TEMPORAL_TLS"] ?? "") == "true" ? new TlsOptions() : null;
            options.Namespace = "default";
            options.Interceptors = new[] { new TracingInterceptor() };
        });
        return services;
    }
}