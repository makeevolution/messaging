using Anko.OrdersService.Core;
using Anko.OrdersService.Core.Entities;
using Anko.OrdersService.Infrastructure.Adapters.Database;
using Anko.OrdersService.Infrastructure.Adapters.Messaging;
using Anko.OrdersService.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Anko.OrdersService.Infrastructure;

// Extension methods to extend IServiceCollection
public static class ServiceExtensions
{
    /* Configure a database to register orders */
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        
        Console.WriteLine("Configuring database...");
        Console.WriteLine(configuration.GetConnectionString("OrdersContext"));
        services.AddDbContext<OrdersDbContext>(opt =>
            opt.UseNpgsql(
                configuration.GetConnectionString("OrdersContext"),
                o => o
                    .SetPostgresVersion(17, 0)));

        services.AddScoped<IOrdersRepository, OrdersRepository>();

        return services;
    }
    
    /*Configure connection to RabbitMQ as well as the publisher */
    public static IServiceCollection ConfigureMessaging(this IServiceCollection services, IConfiguration configuration)
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