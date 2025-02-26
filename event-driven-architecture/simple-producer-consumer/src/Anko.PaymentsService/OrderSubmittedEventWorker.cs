using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Anko.PaymentsService.Adapters;
using Anko.PaymentsService.Core;
using Anko.PaymentsService.Core.ExternalEvents;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;

namespace Anko.PaymentsService;

public class orderSubmittedEventWorker : BackgroundService
{
    const string QUEUE_NAME = "consumer-order-submitted";
    const string DEAD_LETTER_QUEUE_NAME = "consumer-order-submitted-dlq";
    private const string ROUTING_KEY = "order.orderSubmitted";
    private IChannel? _orderSubmittedChannel;
    private HashSet<string> _processedEventIds = new();
    private readonly ActivitySource _source;
    private readonly orderSubmittedEventHandler _handler;
    private readonly IOptions<RabbitMqSettings> _settings;
    private readonly ILogger<orderSubmittedEventWorker> _logger;
    private readonly RabbitMQConnection _connection;
    public orderSubmittedEventWorker(orderSubmittedEventHandler handler, IOptions<RabbitMqSettings> settings, ILogger<orderSubmittedEventWorker> logger, RabbitMQConnection connection)
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
        _orderSubmittedChannel = configuredChannel.Channel;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await _orderSubmittedChannel.BasicConsumeAsync(QUEUE_NAME, false, configuredChannel.Consumer, 
                cancellationToken: stoppingToken);
            
            await Task.Delay(1000, stoppingToken);
        }
    }

    /* Helper method (notice its private) for the background job;
     this is the consumer */
    private async Task Consumer(object model, BasicDeliverEventArgs ea)
    {
        var evtWrapper = await DeserializeIncomingEvent(ea);

        using var instrumentor = InstrumentConsumer(evtWrapper);

        var deliveryCount = ObtainDeliveryCount(ea, instrumentor );
        try
        {
            //Console.WriteLine($"got message {ea.Body}");
            if (_processedEventIds.Contains(evtWrapper.Id))
            {
                instrumentor.AddEvent(new ActivityEvent("Event already processed. Skipping."));
                await _orderSubmittedChannel.BasicAckAsync(ea.DeliveryTag, false);
                return;
            }

            var eventData = evtWrapper.Data as OrderSubmittedEventV1;
            eventData.AddToTelemetry(evtWrapper.Id);

            await _handler.Handle(eventData);
            
            await _orderSubmittedChannel.BasicAckAsync(ea.DeliveryTag, false);
            _processedEventIds.Add(evtWrapper.Id);
        }
        catch (Exception ex)
        {
            instrumentor.AddException(ex);
            _logger.LogError(ex, ex.Message);
                    
            await _orderSubmittedChannel.BasicRejectAsync(ea.DeliveryTag, deliveryCount < 3);
        }
    }

    private Activity? InstrumentConsumer(CloudEvent evtWrapper)
    {
        Activity? instrumentor = null;
        try
        {
            // Start an .NET Activity
            // Get the traceparent from the event wrapper so that this event consuming activity is correlated through OTEL!
            // If the event doesn't (unexpectedly) have a traceparent, just start a new trace...
            var traceparent = evtWrapper.GetPopulatedAttributes()
                .Where(attributeValue => attributeValue.Key.ToString() == "traceparent")
                .Select(attributeValue => attributeValue.Value.ToString()).FirstOrDefault();

            instrumentor = traceparent != null
                ? _source.StartActivity(nameof(orderSubmittedEventWorker), ActivityKind.Internal, ActivityContext.Parse(traceparent, null))
                : _source.StartActivity(nameof(orderSubmittedEventWorker));
            return instrumentor;
        }
        catch
        {
            instrumentor?.Dispose();
            throw;
        }
    }

    private static async Task<CloudEvent> DeserializeIncomingEvent(BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var formatter = new JsonEventFormatter<OrderSubmittedEventV1>();
        var evtWrapper = await formatter.DecodeStructuredModeMessageAsync(new MemoryStream(body), 
            new ContentType("application/json"), null);
        return evtWrapper;
    }

    private static int ObtainDeliveryCount(BasicDeliverEventArgs ea, Activity? processingActivity)
    {
        var deliveryCount = 0;
                
        if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers["x-delivery-count"] != null)
        {
            deliveryCount = int.Parse(ea.BasicProperties.Headers["x-delivery-count"].ToString());
        }

        processingActivity?.AddEvent(new ActivityEvent($"Delivery count is {deliveryCount}"));
        return deliveryCount;
    }
}