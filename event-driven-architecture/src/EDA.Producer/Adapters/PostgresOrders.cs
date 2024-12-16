using System.Text.Json;
using EDA.Producer.Core;

namespace EDA.Producer.Adapters;

public class PostgresOrders(OrdersDbContext context) : IOrders
{
    public async Task<Order> New(string customerId)
    {
        // Create a transaction and only commit later
        var transaction = await context.Database.BeginTransactionAsync();

        var order = new Order()
        {
            CustomerId = customerId,
            OrderId = Guid.NewGuid().ToString()
        };
        // With an outbox storing the event to be published in db before publishing, we are sure that the
        // if db save fails, no events are published, and vice versa.
        var orderOutbox = new OutboxItem() {
            EventTime =  DateTime.UtcNow,
            Processed = false,
            EventData = JsonSerializer.Serialize(new OrderCreatedEvent(){
                OrderId = order.OrderId
            }), // save the event data in the database as a raw json string
            EventType = nameof(OrderCreatedEvent)
        };


        // Create a new order and outboxItem

        await context.Orders.AddAsync(order);
        await context.Outbox.AddAsync(orderOutbox);
        
        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return order;
    }
}