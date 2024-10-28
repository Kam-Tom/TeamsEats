using Microsoft.EntityFrameworkCore;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;

namespace TeamsEats.Infrastructure.Repositories;

internal class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order.Id;
    }

    public async Task DeleteOrderAsync(Order order)
    {
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }

    public async Task<Order> GetOrderAsync(int id)
    {
        var groupOrder = await _context.Orders.Where(g => g.Id == id)
            .Include(g => g.Items).SingleAsync();

        return groupOrder;
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        var groupOrders = await _context.Orders.Include(g => g.Items).ToListAsync();

        return groupOrders;
    }

    public async Task<Order> UpdateOrderAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }

}
