using EDA.Producer.Core;
using Microsoft.EntityFrameworkCore;

namespace EDA.Producer.Adapters;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> context) : base(context){}
    public DbSet<OutboxItem> Outbox { get; set; }
    public DbSet<Order> Orders { get; set; }
}