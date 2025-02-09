using Anko.OrdersService.Core;
using Microsoft.EntityFrameworkCore;

namespace Anko.OrdersService.Adapters;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> context) : base(context){}
    public DbSet<OutboxItem> Outbox { get; set; }
    public DbSet<Order> Orders { get; set; }
}