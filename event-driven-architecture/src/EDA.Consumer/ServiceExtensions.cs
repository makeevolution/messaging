using EDA.Producer.Adapters;
using EDA.Producer.Core;
using Microsoft.EntityFrameworkCore;

namespace EDA.Producer;

public static class ServiceExtensions
{

    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var hostName = configuration["Messaging:HostName"];
        var exchangeName = configuration["Messaging:ExchangeName"];
        var deadLetterExchangeName = configuration["Messaging:DeadLetterExchangeName"];

        if (hostName is null || exchangeName is null)
        {
            throw new EventBusConnectionException("", "Host name is null");
        }
        
        // Configure the DI container to add rabbit mq related services/implementations
        // Add a singleton that will give the rabbitmqconnection everytime RabbitMQConnection is made as argument to function
        services.AddSingleton(new RabbitMQConnection(hostName!, exchangeName!, deadLetterExchangeName!));
        // For below, see IOptions in https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-9.0
        // The RabbitMQ settings we defined in "Messaging" section will be available as a class for easy access.
        // This is similar to Config class of our Django app, difference being this is injected by framework instead of us instantiating the class
        // everywhere uncleanly
        services.Configure<RabbitMqSettings>(configuration.GetSection("Messaging"));

        return services;
    }
}