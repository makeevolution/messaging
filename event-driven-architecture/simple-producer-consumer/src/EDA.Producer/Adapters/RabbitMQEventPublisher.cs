using System.Diagnostics;
using System.Text;
using System.Text.Json;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using EDA.Producer.Core;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EDA.Producer.Adapters;

public class RabbitMQEventPublisher : IEventPublisher
{
    private readonly RabbitMqSettings _rabbitMqSettings;
        private const string SOURCE = "http://com.orders";

    private readonly IConnection _connection;
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    
    public RabbitMQEventPublisher(IOptions<RabbitMqSettings> settings, ILogger<RabbitMQEventPublisher> logger, RabbitMQConnection connection)
    {
        // See ConfigureMessaging class for explanation of IOptions
        _logger = logger;
        _connection = connection.Connection;
        _rabbitMqSettings = settings.Value;
    }
    
    public async Task Publish(OrderCreatedEvent evt)
    {
        var eventName = "order-created";
        var channel = await _connection.CreateChannelAsync();
        
        //var body = JsonSerializer.SerializeToUtf8Bytes(evt);
        var evtWrapper = new CloudEvent
        {
            Type = eventName,
            Source = new Uri(SOURCE),
            Time = DateTimeOffset.Now,
            DataContentType = "application/json",
            Id = Guid.NewGuid().ToString(),
            Data = evt,
        };
        
        var evtFormatter = new JsonEventFormatter();

        var json = evtFormatter.ConvertToJsonElement(evtWrapper).ToString();
        var body = Encoding.UTF8.GetBytes(json);
        
        this._logger.LogInformation($"Publishing '{eventName}' to '{_rabbitMqSettings.ExchangeName}'");
        this._logger.LogInformation(json);
        
        //put the data on to the product queue
        await channel.BasicPublishAsync(exchange: _rabbitMqSettings.ExchangeName, routingKey: eventName, body: body);
    }
}