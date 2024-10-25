using TeamsEats.Domain.Models;

namespace TeamsEats.Domain.Interfaces;
public interface IOrderItemsRepository
{
    public Task<int> CreateOrderItemAsync(OrderItem orderItem);
    public Task UpdateOrderItemAsync(OrderItem orderItem);
    public Task<OrderItem> GetOrderItemAsync(int orderItemId);
    public Task DeleteOrderItemAsync(OrderItem orderItem);
}