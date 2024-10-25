using Microsoft.EntityFrameworkCore;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;

namespace TeamsEats.Infrastructure.Repositories;

internal class OrderItemsRepository : IOrderItemsRepository
{
    AppDbContext _context;
    public OrderItemsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateOrderItemAsync(OrderItem orderItem)
    {
        _context.OrderItems.Add(orderItem);
        await _context.SaveChangesAsync();
        return orderItem.Id;
    }

    public async Task DeleteOrderItemAsync(OrderItem orderItem)
    {
        _context.OrderItems.Remove(orderItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOrderItemAsync(OrderItem orderItem)
    {
        _context.OrderItems.Update(orderItem);
        await _context.SaveChangesAsync();
    }

    public async Task<OrderItem> GetOrderItemAsync(int orderItemId)
    {
        var orderItem = await _context.OrderItems.Where(x => x.Id == orderItemId).Include(o => o.GroupOrder).SingleAsync();
        return orderItem;
    }
}
