namespace Anko.OrdersService.Core.Entities;

public interface IOrders
{
    Task<Order> New(string customerId);
}