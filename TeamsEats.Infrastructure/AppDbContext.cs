using Microsoft.EntityFrameworkCore;
using TeamsEats.Domain.Models;

namespace TeamsEats.Infrastructure;

internal class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<ItemEntity> Items { get; set; }

}
