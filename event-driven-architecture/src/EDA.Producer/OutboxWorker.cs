using System.Text.Json;
using EDA.Producer.Adapters;
using EDA.Producer.Core;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace EDA.Producer;

// Create a tasks.py (background job) that will get outbox items from the db periodically and then publish the event
public class OutboxWorker(ILogger<OutboxWorker> logger, IServiceScopeFactory serviceScopeFactory, IEventPublisher eventPublisher) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // get the events with processed false from the outbox db table
                var dbService = scope.ServiceProvider.GetRequiredService<OrdersDbContext>(); // Use GetRequiredService instead of GetService so we get exception if service does not exist
                var unprocessedOutboxItems = dbService.Outbox.Where(item => !item.Processed);
                logger.LogInformation("\n\n\nThere are {OutboxCount} unprocessed items in the outbox", unprocessedOutboxItems.Count());
                foreach (var item in unprocessedOutboxItems)
                {
                    switch (item.EventType)
                    {
                        case nameof(OrderCreatedEvent):
                            {
                                var eventDataString = item.EventData; // A JsonString

                                logger.LogDebug("Publishing data {data} to event bus", eventDataString);
                                await eventPublisher.Publish(
                                    JsonSerializer.Deserialize<OrderCreatedEvent>(eventDataString)
                                );
                                item.Processed = true;
                                dbService.Outbox.Update(item);
                                break;
                            }
                        default:
                            {
                                logger.LogInformation("Unknown event type '{EventType}'", item.EventType);
                                break;
                            }
                    }
                }
                await dbService.SaveChangesAsync(stoppingToken);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}