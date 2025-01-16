using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace shared
{
    public static class Setup
    {
        private const string OTEL_DEFAULT_GRPC_ENDPOINT = "http://localhost:4317";
        
        public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services,
            IConfiguration configuration, string applicationName, string[]? additionalSources = null)
        {
            services.AddLogging();
            
            // The AddOpenTelemetry below is supplied by OpenTelemetry.Extensions.Hosting package
            var otel = services.AddOpenTelemetry();
            otel.ConfigureResource(resource => resource
                .AddService(serviceName: applicationName));
            
            otel.WithTracing(tracing =>  // Configure tracing
            {
                tracing.AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = new Func<HttpContext, bool>((httpContext) =>
                    {
                        try
                        {
                            if (httpContext.Request.Path.Value.Contains("/notifications"))
                            {
                                return false;
                            }

                            return true;
                        }
                        catch
                        {
                            return true;
                        }
                    });
                });
                tracing.AddGrpcClientInstrumentation();
                tracing.AddHttpClientInstrumentation();
                tracing.AddSource(applicationName);

                if (additionalSources != null)
                {
                    foreach (var source in additionalSources)
                    {
                        tracing.AddSource(source);
                    }
                }
                
                tracing.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? OTEL_DEFAULT_GRPC_ENDPOINT);
                });
            });
            
            services.AddHttpContextAccessor();
            return services;
        }
    }
}