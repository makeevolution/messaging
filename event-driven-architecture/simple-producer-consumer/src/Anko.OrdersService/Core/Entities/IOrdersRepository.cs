namespace Anko.OrdersService.Core.Entities;

public interface IOrdersRepository
{
    Task<Order> Submit(string orderId);
    Task<Order> RetrieveOrder(string orderId);
}