using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using EDA.Consumer.Adapters;
using EDA.Consumer.Core;
using EDA.Consumer.Core.ExternalEvents;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;

namespace EDA.Consumer;

public class OrderCreatedEventWorker : BackgroundService
{
    const string QUEUE_NAME = "consumer-order-created";
    const string DEAD_LETTER_QUEUE_NAME = "consumer-order-created-dlq";
    private const string ROUTING_KEY = "order-created";
    private IChannel? _orderCreatedChannel;
    private HashSet<string> _processedEventIds = new();
    private readonly ActivitySource _source;
    private readonly OrderCreatedEventHandler _handler;
    private readonly IOptions<RabbitMqSettings> _settings;
    private readonly ILogger<OrderCreatedEventWorker> _logger;
    private readonly RabbitMQConnection _connection;
    public OrderCreatedEventWorker(OrderCreatedEventHandler handler, IOptions<RabbitMqSettings> settings, ILogger<OrderCreatedEventWorker> logger, RabbitMQConnection connection)
    {
        _handler = handler;
        _settings = settings;
        _logger = logger;
        _connection = connection;
        _source = new ActivitySource(ApplicationDefaults.ServiceName);
    }

    /* The actual background job that is periodically executed */
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Create a configured channel to process messages.
        // A configured channel is something that consists of 2 things: 
        // 1. A channel with queues and its bindings established
        // 2. A consumer i.e. event handler that will do something upon messages coming into the queue'
        var channelConfiguration = new ChannelConfig(_settings.Value.ExchangeName, 
            QUEUE_NAME, ROUTING_KEY, _settings.Value.DeadLetterExchangeName, DEAD_LETTER_QUEUE_NAME, Consumer);
        var configuredChannel = await _connection.CreateAndConfigureConsumingChannel(channelConfiguration, stoppingToken);
        _orderCreatedChannel = configuredChannel.Channel;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await _orderCreatedChannel.BasicConsumeAsync(QUEUE_NAME, false, configuredChannel.Consumer, 
                cancellationToken: stoppingToken);
            
            await Task.Delay(1000, stoppingToken);
        }
    }

    /* Helper method (notice its private) for the background job;
     this is the consumer */
    private async Task Consumer(object model, BasicDeliverEventArgs ea)
    {
        var deliveryCount = 0;
                
        if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers["x-delivery-count"] != null)
        {
            deliveryCount = int.Parse(ea.BasicProperties.Headers["x-delivery-count"].ToString());
        }
            
        _logger.LogInformation("Delivery count is {deliveryCount}", deliveryCount);
            
        try
        {
            var body = ea.Body.ToArray();
            var formatter = new JsonEventFormatter<OrderCreatedEvent>();
            var evtWrapper = await formatter.DecodeStructuredModeMessageAsync(new MemoryStream(body), 
                new ContentType("application/json"), new List<CloudEventAttribute>(0));
            
            if (_processedEventIds.Contains(evtWrapper.Id))
            {
                _logger.LogInformation("Event already processed. Skipping.");
                await _orderCreatedChannel.BasicAckAsync(ea.DeliveryTag, false);
                return;
            }
            
            // Get the traceparent from the event wrapper, and start an .NET Activity, so that this event consuming activity is correlated through OTEL!
            var traceparent = evtWrapper.GetPopulatedAttributes()
                .Where(attributeValue => attributeValue.Key.ToString() == "traceparent")
                .Select(attributeValue => attributeValue.Value.ToString()).FirstOrDefault();
            if (traceparent != null)
            {
                var context = ActivityContext.Parse(traceparent, null);  // parse the traceparent to a .NET context
                using var processingActivity = _source.StartActivity("process222", ActivityKind.Internal, context);
                
            }
            await _handler.Handle(evtWrapper.Data as OrderCreatedEvent);
            
            await _orderCreatedChannel.BasicAckAsync(ea.DeliveryTag, false);
            _processedEventIds.Add(evtWrapper.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $" [x] Failure processing message. {ex.Message}");
                    
            await _orderCreatedChannel.BasicRejectAsync(ea.DeliveryTag, deliveryCount < 3);
        }
    }
}