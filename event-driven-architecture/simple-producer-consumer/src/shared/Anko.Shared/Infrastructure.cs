using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Anko.Shared.Authentication;
namespace Anko.Shared
{
    /* Extension method to apply shared infrastructure to all microservices */
    public static class Infrastructure
    {

        /* Create an extension method to add Serilog and OTEL */
        public static IHostBuilder AddSharedInfrastructure(this IHostBuilder hostBuilder,
            IConfiguration configuration, string applicationName)
        {
            // Since we use serilog, we can disable the below
            // services.AddLogging();
            
            hostBuilder.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));
            
            hostBuilder.ConfigureServices((context, services) =>
            {
                ConfigureOpenTelemetry(services, configuration, applicationName);
                ConfigureCORSPolicies(services);
                services.AddHttpContextAccessor();
            });
            return hostBuilder;
        }
        
        /* Customize CORS policies that can be used after app.run() */
        private static void ConfigureCORSPolicies(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: CorsSettings.ALLOW_ALL_POLICY_NAME,
                    policy  =>
                    {
                        policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });
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
            // Now with Serilog, Seq and set up, all code that has an Activity.Current not being null, wil also have all
            // logger.LogInformation(...) we do will be captured and correlated in Seq!
            
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
                // Most important in configuring otlpExporter is to set the otel endpoint and the otel protocol
                // These are set up through the environment variables OTEL_EXPORTER_OTLP_ENDPOINT and OTEL_EXPORTER_OTLP_PROTOCOL
                // You can specify through the docker-compose, or through appsettings.json;
                // Remember that the value in docker-compose wins!
                // The below call will automatically read the env vars above and export telemetry data to the endpoint when
                // an Activity ends.
                tracing.AddOtlpExporter();
                // The above is exactly the same as doing the below:
                // tracing.AddOtlpExporter((otlpOptions) =>
                // {
                //    otlpOptions.Endpoint = new Uri(configuration.GetValue<string>("OTEL_EXPORTER_OTLP_ENDPOINT"));
                //    otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                // };
            });
        }
    }
}