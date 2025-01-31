using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared
{
    /* Extension method to apply shared infrastructure to all microservices */
    public static class Infrastructure
    {
        private const string OTEL_DEFAULT_GRPC_ENDPOINT = "http://localhost:4317"; //CHANGE TO JAEGER HOST FOR DOCKER!
        
        public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services,
            IConfiguration configuration, string applicationName)
        {
            services.AddLogging();

            ConfigureOpenTelemetry(services, configuration, applicationName);

            services.AddHttpContextAccessor();
            return services;
        }
        
        /* OpenTelemetry configuration */
        private static void ConfigureOpenTelemetry(IServiceCollection services, IConfiguration configuration, string applicationName)
        {
            
            // How does OTEL work with .NET?
            // .NET has a distributed tracing library called System.Diagnostics.Activity. Read below for more info.
            // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-concepts
            // You can create a new activity using ActivitySource, see OutboxWorker.cs constructor.
            // The name of the source is usually the application name.
            // To start an activity within the source, use StartActivity, see OutboxWorker.cs too.
            // However, StartActivity will return null if there is no ActivityListener attached to the source!
            // Usually, we would add an ActivityListener as shown as an example:
            // using System;
            // using System.Diagnostics;
            //
            // class Program
            // {
            //     static void Main()
            //     {
            //         // Create an ActivitySource
            //         var activitySource = new ActivitySource("MyAppActivitySource");
            //
            //         // Create an ActivityListener
            //         var activityListener = new ActivityListener
            //         {
            //             // Specify the activity source name to listen for
            //             ShouldListenTo = (source) => source.Name == "MyAppActivitySource",
            //
            //             // Define actions when an activity is started
            //             ActivityStarted = (activity) => 
            //             {
            //                 Console.WriteLine($"Activity started: {activity.DisplayName}");
            //             },
            //
            //             // Define actions when an activity is stopped
            //             ActivityStopped = (activity) =>
            //             {
            //                 Console.WriteLine($"Activity stopped: {activity.DisplayName}");
            //             }
            //         };
            //
            //         // Register the ActivityListener
            //         ActivitySource.AddActivityListener(activityListener);
            //
            //         // Start an activity
            //         using (var activity = activitySource.StartActivity("MyActivity"))
            //         {
            //             // Simulate some work being done
            //             Console.WriteLine("Activity is running...");
            //         }
            //
            //         // The activity has been stopped, and the listener will handle the stopping event
            //     }
            // }
            // But, with OTEL, In tracing.AddSource line below, OTEL will automatically create the listener to the source!
            // Note that, without a listener, activitySource.StartActivity("MyActivity") above will return null!
            
            // Some more notes:
            // An API instrumented by OTEL will:
            // - For any outgoing request (e.g. a GET request call to another API), it will add a traceparent header with value the same format as above
            // - For any incoming request, it will look for traceparent header from the request, grab the trace ID from that header,
            // generate a new span for this new request, and send this to Jaeger for correlation (i.e. correlate this request with the request this request came from!)
            
            // In .NET, you can get the current activity in the thread using the call Activity.Current (note this can be null!)
            // This is useful if you don't have access to the activity variable.
            
            // An activity has an ID field. It nicely has the same format as W3C traceparent e.g.
            // 00-19e89a2e7b4096ece7ddcd78821a0397-518c033d28cb90a0-01. Read the W3C traceparent for more info.
            // Look at OutboxWorker.cs; this ID is saved into the Outbox, and we can create an Activity by grabbing this
            // ID and thus correlate and instrument our worker with the request that wanted to publish the event!
            
            // Everytime you do something with Activity e.g. Activity.Current?.AddEvent(new ActivityEvent("test123"));
            // It will be captured by OTEL. For example with AddEvent, it will be become a Log in Jaeger UI. Try it out.
            
            // An API endpoint in .NET will have Activity.Current by default! So you don't need to do new
            // ActivitySource(); this is then also the reason how each endpoint is automatically instrumented by OTEL!
            
            var otel = services.AddOpenTelemetry();  // AddOpenTelemetry comes from OpenTelemetry.Extensions.Hosting package

            otel.ConfigureResource(resource => resource
                .AddService(serviceName: applicationName));
            
            otel.WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation();
                tracing.AddGrpcClientInstrumentation();
                tracing.AddHttpClientInstrumentation();
                // The following configures OTEL to create an ActivityListener on all ActivitySource with the name applicationName!
                // This is important! Otherwise any activity started by this source will return null (since there is no listener to it!)
                tracing.AddSource(applicationName);
                tracing.AddConsoleExporter();
                tracing.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? OTEL_DEFAULT_GRPC_ENDPOINT);
                });
            });
        }
    }
}