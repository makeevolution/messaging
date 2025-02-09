using System.Text.Json;
using Anko.OrdersService.Core;

namespace Anko.OrdersService.Adapters;

public class PostgresOrders(OrdersDbContext context) : IOrders
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
    public async Task<Order> New(string customerId)
    {
        // Create a transaction and only commit later
        var transaction = await context.Database.BeginTransactionAsync();

        var order = new Order()
        {
            CustomerId = customerId,

            OrderId = customerId == "error" ? $"6{Guid.NewGuid().ToString()}" : Guid.NewGuid().ToString()
        };
        // With an outbox storing the event to be published in db before publishing, we are sure that the
        // if db save fails, no events are published, and vice versa.
        var orderOutbox = new OutboxItem() {
            Processed = false,
            EventData = JsonSerializer.Serialize(new OrderCreatedEventV1(){
                OrderId = order.OrderId
            }), // save the event data in the database as a raw json string
            EventType = nameof(OrderCreatedEventV1)
        };

        // Create a new order and outboxItem

        await context.Orders.AddAsync(order);
        await context.Outbox.AddAsync(orderOutbox);
        
        // Commit together, ensuring atomicity
        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return order;
    }
}