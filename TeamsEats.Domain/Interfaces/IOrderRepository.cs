using TeamsEats.Domain.Models;

namespace TeamsEats.Domain.Interfaces;

/// <summary>
/// Repository interface for managing orders.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="order">The order to create.</param>
    /// <returns>The ID of the created order.</returns>
    Task<int> CreateOrderAsync(Order order);

    /// <summary>
    /// Deletes an order.
    /// </summary>
    /// <param name="order">The order to delete.</param>
    Task DeleteOrderAsync(Order order);

    /// <summary>
    /// Retrieves an order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order.</param>
    /// <returns>The order, if found.</returns>
    Task<Order> GetOrderAsync(int id);

    /// <summary>
    /// Retrieves all orders.
    /// </summary>
    /// <returns>A collection of all orders.</returns>
    Task<IEnumerable<Order>> GetOrdersAsync();

    /// <summary>
    /// Updates an existing order.
    /// </summary>
    /// <param name="order">The order to update.</param>
    /// <returns>The updated order.</returns>
    Task UpdateOrderAsync(Order order);

    /// <summary>
    /// Retrieves an item by its ID.
    /// </summary>
    /// <param name="itemId">The ID of the item.</param>
    /// <returns>The requested item, if found.</returns>
    Task<Item> GetItemAsync(int itemId);
}