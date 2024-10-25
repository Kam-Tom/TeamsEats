using Microsoft.EntityFrameworkCore;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;

namespace TeamsEats.Infrastructure.Repositories;

internal class GroupOrderRepository : IGroupOrderRepository
{
    private readonly AppDbContext _context;
    public GroupOrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateGroupOrderAsync(GroupOrder groupOrder)
    {
        _context.GroupOrders.Add(groupOrder);
        await _context.SaveChangesAsync();
        return groupOrder.Id;
    }

    public async Task DeleteGroupOrderAsync(GroupOrder groupOrder)
    {
        _context.GroupOrders.Remove(groupOrder);
        await _context.SaveChangesAsync();
    }

    public async Task<GroupOrder> GetGroupOrderAsync(int groupOrderId)
    {
        var groupOrder = await _context.GroupOrders.Where(g => g.Id == groupOrderId)
            .Include(g => g.OrderItems).SingleAsync();

        return groupOrder;
    }

    public async Task<IEnumerable<GroupOrder>> GetGroupsOrdersAsync()
    {
        var groupOrders = await _context.GroupOrders.Include(g => g.OrderItems).ToListAsync();

        return groupOrders;
    }

    public async Task<GroupOrder> UpdateGroupOrderAsync(GroupOrder groupOrder)
    {
        _context.GroupOrders.Update(groupOrder);
        await _context.SaveChangesAsync();
        return groupOrder;
    }

}
