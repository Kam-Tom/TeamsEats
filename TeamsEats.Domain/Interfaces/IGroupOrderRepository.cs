using TeamsEats.Domain.Models;

namespace TeamsEats.Domain.Interfaces;
public interface IGroupOrderRepository
{
    Task<int> CreateGroupOrderAsync(GroupOrder groupOrder);
    Task DeleteGroupOrderAsync(GroupOrder groupOrder);
    Task<GroupOrder> GetGroupOrderAsync(int groupOrderId);
    Task<IEnumerable<GroupOrder>> GetGroupsOrdersAsync();
    Task<GroupOrder> UpdateGroupOrderAsync(GroupOrder groupOrder);
}
