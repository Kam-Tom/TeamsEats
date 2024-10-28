using Microsoft.EntityFrameworkCore;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;

namespace TeamsEats.Infrastructure.Repositories;

internal class ItemsRepository : IItemRepository
{
    AppDbContext _context;
    public ItemsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateItemAsync(Item item)
    {
        _context.Items.Add(item);
        await _context.SaveChangesAsync();
        return item.Id;
    }

    public async Task DeleteItemAsync(Item item)
    {
        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(Item item)
    {
        _context.Items.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task<Item> GetItemAsync(int id)
    {
        var item = await _context.Items.Where(x => x.Id == id).Include(o => o.Order).SingleAsync();
        return item;
    }
}
