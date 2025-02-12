using Anko.OrdersService.Core;
using Anko.OrdersService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anko.OrdersService.Infrastructure.Adapters.Database;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> context) : base(context){}
    public DbSet<OutboxItem> Outbox { get; set; }
    public DbSet<Order> Orders { get; set; }
}