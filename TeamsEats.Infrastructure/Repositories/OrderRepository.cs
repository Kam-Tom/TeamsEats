using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;

namespace TeamsEats.Infrastructure.Repositories;

internal class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public OrderRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> CreateOrderAsync(Order order)
    {
        var orderEnity = _mapper.Map<OrderEntity>(order);
        _context.Orders.Add(orderEnity);
        await _context.SaveChangesAsync();
        return orderEnity.Id;
    }

    public async Task DeleteOrderAsync(Order order)
    {
        var orderEnity = await _context.Orders.SingleAsync(o => o.Id == order.Id);
        _context.Orders.Remove(orderEnity);
        await _context.SaveChangesAsync();
    }

    public async Task<Order> GetOrderAsync(int id)
    {
        var orderEntities = await _context.Orders.Where(g => g.Id == id).SingleAsync();

        var orders = _mapper.Map<Order>(orderEntities);

        return orders;
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        var orderEntities = await _context.Orders.ToListAsync();

        var orders = _mapper.Map<IEnumerable<Order>>(orderEntities);

        return orders;
    }

    public async Task UpdateOrderAsync(Order order)
    {
        var orderEnity = await _context.Orders.FindAsync(order.Id);
        _mapper.Map(order, orderEnity);
        await _context.SaveChangesAsync();

    }

    public async Task<Item> GetItemAsync(int itemId)
    {
        var itemEntity = await _context.Items.SingleAsync(i => i.Id == itemId);
        var item = _mapper.Map<Item>(itemEntity);
        return item;
    }

}
