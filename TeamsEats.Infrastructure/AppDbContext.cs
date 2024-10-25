using Microsoft.EntityFrameworkCore;
using TeamsEats.Domain.Models;

namespace TeamsEats.Infrastructure;

internal class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }
    public DbSet<GroupOrder> GroupOrders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}
