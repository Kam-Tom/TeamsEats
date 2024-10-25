
namespace TeamsEats.Domain.Services;

public interface IGraphService
{
    Task<string> GetPhoto(string userId);
    Task<string> GetUserID();
    Task<string> GetUserDisplayName(string userId);
    Task SendMessage(string addresserId, string addresseeId, string message);
    Task SendActivityFeedTypeClosed(string addresserId, string addresseeId, string restaurant, int groupOrderId);
    Task SendActivityFeedTypeDeleted(string addresserId, string addresseeId, string restaurant);
    Task SendActivityFeedTypeDelivered(string addresserId, string addresseeId, int groupOrderId);
}
