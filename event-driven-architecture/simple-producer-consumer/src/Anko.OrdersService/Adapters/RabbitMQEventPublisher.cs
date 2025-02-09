using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Anko.OrdersService.Core;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Anko.OrdersService.Adapters;

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
    
    public async Task Publish(OrderCreatedEventV1 evt)
    {
        var eventName = evt.EventName;
        var channel = await _connection.CreateChannelAsync();
        
        var evtWrapper = new CloudEvent
        {
            Type = eventName,
            Source = new Uri(SOURCE),
            Time = DateTimeOffset.Now,
            DataContentType = "application/json",
            Id = Guid.NewGuid().ToString(),  // THIS IS THE EVENT ID, NOT AN ID RELATED TO TRACING!!!
            // THIS ID IS PRIMARILY USED TO HELP CONSUMER IMPLEMENT IDEMPOTENCY; NOTHING TO DO WITH OTEL/TRACING!
            Data = evt,
        };
        evt.AddToTelemetry(evtWrapper.Id);  // Remember, Id here is the event ID
        evtWrapper.SetAttributeFromString("traceparent", Activity.Current?.Id);  // This ID is the OTEL related ID!
        var evtFormatter = new JsonEventFormatter();

        var json = evtFormatter.ConvertToJsonElement(evtWrapper).ToString();
        var body = Encoding.UTF8.GetBytes(json);

        var msg = $"Publishing to '{_rabbitMqSettings.ExchangeName}'";
        // The log msg below is a contrived example.
        // If exporting to Jaeger, only AddEvent will show up there
        // If exporting to Seq, both logger and AddEvent will show up!
        Activity.Current?.AddEvent(new ActivityEvent(msg));
        _logger.LogInformation(msg);
        
        await channel.BasicPublishAsync(exchange: _rabbitMqSettings.ExchangeName, routingKey: eventName, body: body);
    }
}