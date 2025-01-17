using System.Diagnostics;
using System.Text.Json;
using EDA.Producer.Adapters;
using EDA.Producer.Core;

namespace EDA.Producer;

// Create a tasks.py (background job) that will get outbox items from the db periodically and then publish the event
public class OutboxWorker : BackgroundService
{
    private readonly ILogger<OutboxWorker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IEventPublisher _eventPublisher;
    private readonly ActivitySource _source;
    public OutboxWorker(ILogger<OutboxWorker> logger, IServiceScopeFactory serviceScopeFactory, IEventPublisher eventPublisher)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _eventPublisher = eventPublisher;
        // Note how the source name below MUST BE the same as the one in OTEL setup!
        // Otherwise no one is listening to the source and StartActivity will return null!
        _source = new ActivitySource(ApplicationDefaults.ServiceName);  
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (IServiceScope scope = _serviceScopeFactory.CreateScope())
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // get the events with processed false from the outbox db table
                var dbService = scope.ServiceProvider.GetRequiredService<OrdersDbContext>(); // Use GetRequiredService instead of GetService so we get exception if service does not exist
                var unprocessedOutboxItems = dbService.Outbox.Where(item => !item.Processed);
                _logger.LogInformation("\n\n\nThere are {OutboxCount} unprocessed items in the outbox", unprocessedOutboxItems.Count());
                
                foreach (var item in unprocessedOutboxItems)
                {
                    using var processingActivity = CreateActivity(item);
                    // using is very important, so that everytime we do an activity, the activity is stopped auto!
                    // Otherwise, OTEL will never process the activity (e.g. send to Jaeger) since the activity is never stopped!
                    // Having using is equivalent to the following:
                    // var processingActivity = CreateActivity(item); // Notice there is no using here!
                    // try
                    // {
                    //     processingActivity?.SetTag("exampleTag", "value");
                    // }
                    // finally
                    // {
                    //     processingActivity?.Stop();  // Explicitly stop the activity, and OTEL will then be able to process the activity!
                    // }
                    // If we access the activity through Activity.Current?, it will also nicely be automatically disposed!
                    switch (item.EventType)
                    {
                        case nameof(OrderCreatedEvent):
                        {
                            processingActivity?.AddEvent(new ActivityEvent(
                                $"Publishing data {item.EventData} to event bus"));
                                // Publish the event
                                var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(item.EventData);
                                await _eventPublisher.Publish(evt);
                                item.Processed = true;
                                dbService.Outbox.Update(item);
                                break;
                            }
                        default:
                            {
                                _logger.LogInformation("Unknown event type '{EventType}'", item.EventType);
                                break;
                            }
                    }
                }
                await dbService.SaveChangesAsync(stoppingToken);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
    
    /* Instrument the worker so that it is correlated in Jaeger with the request that wanted to publish the event.
     See Infrastructure.cs for more info on how this works*/
    private Activity? CreateActivity(OutboxItem outboxItem)
    {
        if (!string.IsNullOrEmpty(outboxItem.TraceParent))
        {
            try
            {
                var context = ActivityContext.Parse(outboxItem.TraceParent, null);  // Grab the parent
                return _source.StartActivity("process", ActivityKind.Internal, context);
                // Now OTEL has instrumented this worker, and it will show up correlated with the parent, in Jaeger.
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failure parsing tracecontext from outbox item");
            }
        }

        return null;
    }
}