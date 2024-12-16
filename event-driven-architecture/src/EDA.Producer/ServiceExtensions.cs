using EDA.Producer.Adapters;
using EDA.Producer.Core;
using Microsoft.EntityFrameworkCore;

namespace EDA.Producer;

public static class ServiceExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrdersDbContext>(opt =>
            opt.UseNpgsql(
                configuration.GetConnectionString("OrdersContext"),
                o => o
                    .SetPostgresVersion(17, 0)));

        services.AddScoped<IOrders, PostgresOrders>();

        return services;
    }
    
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var hostName = configuration["Messaging:HostName"];
        var exchangeName = configuration["Messaging:ExchangeName"];

        if (hostName is null || exchangeName is null)
        {
            throw new EventBusConnectionException("", "Host name is null");
        }
        
        // Configure the DI container to add rabbit mq related services/implementations
        // Add a singleton that will give the rabbitmqconnection everytime RabbitMQConnection is made as argument to function
        services.AddSingleton(new RabbitMQConnection(hostName!, exchangeName!));
        // For below, see IOptions in https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-9.0
        // The RabbitMQ settings we defined in "Messaging" section will be available as a class for easy access.
        // This is similar to Config class of our Django app, difference being this is injected by framework instead of us instantiating the class
        // everywhere uncleanly
        services.Configure<RabbitMqSettings>(configuration.GetSection("Messaging"));
        // Add a singleton that will be injected when an IEventPublisher is made as argument to a function
        services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();

        return services;
    }
}