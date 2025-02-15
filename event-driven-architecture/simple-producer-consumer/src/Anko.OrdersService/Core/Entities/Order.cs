namespace Anko.OrdersService.Core.Entities;

public class Order
{
    public string Id { get; set; }

    public string CustomerId { get; set; } = Guid.NewGuid().ToString();
    
    public bool Submitted { get; set; } = false;
}