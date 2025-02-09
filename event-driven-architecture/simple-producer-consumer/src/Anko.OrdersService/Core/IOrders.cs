namespace Anko.OrdersService.Core;

public interface IOrders
{
    Task<Order> New(string customerId);
}