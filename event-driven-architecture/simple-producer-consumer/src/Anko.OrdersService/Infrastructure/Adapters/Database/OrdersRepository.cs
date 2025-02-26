using System.Text.Json;
using Anko.OrdersService.Core.Entities;
using Anko.OrdersService.Core.Events;
using Microsoft.EntityFrameworkCore;

namespace Anko.OrdersService.Infrastructure.Adapters.Database;

public class OrdersRepository(OrdersDbContext context, ILogger<OrdersRepository> logger) : IOrdersRepository
{
    /* Handle new orders
     Note that the function is rigged; if the customerId is supplied as "error", then 
     the orderId will starts with 6. Then see the consumer code; orderId that starts with 6 will be thrown
     to the dead letter queue.
     This is to illustrate the dead letter queue error handling i.e. unprocessable messages should not
     be discarded but stored somewhere for investigation!
     
     Also notice we are using the Outbox pattern here; we do not publish the event directly after writing data to db
     but let a background worker do it for us periodically
     
     Benefits are:
     - Reliability: If the event publishing fails e.g. network problems, it shall be retried always.
     - Atomicity: Event publishing and writing data to db can't be done atomically. With this pattern, we use
     transactions that ensure event publishing and db write is all-or-nothing.
     
     */
    public async Task<Order> Submit(string orderId)
    {
        // Create a transaction and only commit later
        
        var transaction = await context.Database.BeginTransactionAsync();

        // Find the order to be submitted, change its state and then create the outboxItem

        var order = await RetrieveOrder(orderId);

        // With an outbox storing the event to be published in db before publishing, we are sure that the
        // if db save fails, no events are published, and vice versa.
        var orderOutbox = new OutboxItem() {
            Processed = false,
            EventData = JsonSerializer.Serialize(new OrderSubmittedEventV1(){
                OrderId = order.Id
            }), // save the event data in the database as a raw json string
            EventType = nameof(OrderSubmittedEventV1)
        };

        order.Submitted = true;
        await context.Outbox.AddAsync(orderOutbox);
        
        // Commit together, ensuring atomicity
        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return order;
    }

    public async Task<Order> RetrieveOrder(string orderId)
    {
        var order = await context.Orders.FirstOrDefaultAsync(ordr => ordr.Id == orderId);
        if (order == null)
        {
            logger.LogInformation($"Order with id: '{orderId}' not found! Generating a random order with random data for temporary demo purposes");
            order = new Order(){Id = Guid.NewGuid().ToString()};
        }
        return order;
    }
}