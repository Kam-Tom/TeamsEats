using TeamsEats.Domain.Models;

namespace TeamsEats.Domain.Interfaces;

/// <summary>
/// Repository interface for managing items.
/// </summary>
public interface IItemRepository
{
    /// <summary>
    /// Creates a new item.
    /// </summary>
    /// <param name="item">The item to create.</param>
    /// <returns>The ID of the created item.</returns>
    Task<int> CreateItemAsync(Item item);

    /// <summary>
    /// Updates an existing item.
    /// </summary>
    /// <param name="item">The item to update.</param>
    Task UpdateItemAsync(Item item);

    /// <summary>
    /// Retrieves an item by its ID.
    /// </summary>
    /// <param name="id">The ID of the item.</param>
    /// <returns>The requested item, if found.</returns>
    Task<Item> GetItemAsync(int id);

    /// <summary>
    /// Deletes an item.
    /// </summary>
    /// <param name="item">The item to delete.</param>
    Task DeleteItemAsync(Item item);
}
