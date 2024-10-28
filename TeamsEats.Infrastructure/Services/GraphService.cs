using Microsoft.Graph;
using TeamsEats.Domain.Services;

namespace TeamsEats.Infrastructure.Services;

internal class GraphService : IGraphService
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly string _appId = "7e0988e5-128c-4a09-a6d9-036685c900b0";

    public GraphService(GraphServiceClient graphServiceClient)
    {
        _graphServiceClient = graphServiceClient;
    }

    public async Task<string> GetPhoto(string userId)
    {
        try
        {
            var photoStream = await _graphServiceClient.Users[userId].Photo.Content.Request().GetAsync();
            using var memoryStream = new MemoryStream();
            await photoStream.CopyToAsync(memoryStream);
            var photoBytes = memoryStream.ToArray();
            return Convert.ToBase64String(photoBytes);
        }
        catch (ServiceException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return string.Empty;
        }
    }

    public async Task<string> GetUserId()
    {
        var user = await _graphServiceClient.Me.Request().GetAsync();
        return user.Id;
    }

    public async Task<string> GetUserDisplayName(string userId)
    {
        var user = await _graphServiceClient.Users[userId].Request().GetAsync();
        return user.DisplayName;
    }

    public async Task SendMessage(string addresserId, string addresseeId, string message)
    {
        var members = new ChatMembersCollectionPage
        {
            CreateChatMember(addresserId),
            CreateChatMember(addresseeId)
        };

        var chat = await _graphServiceClient.Chats.Request().AddAsync(new Chat
        {
            ChatType = ChatType.OneOnOne,
            Members = members
        });

        var chatMessage = new ChatMessage
        {
            Body = new ItemBody { Content = message }
        };

        await _graphServiceClient.Chats[chat.Id].Messages.Request().AddAsync(chatMessage);
    }

    public async Task SendActivityFeedTypeClosed(string addresserId, string addresseeId, string restaurant, int orderId)
    {
        await SendActivityNotification(addresseeId, "orderClosed", "Everyone is waiting for your payment.",
            $"https://teams.microsoft.com/l/entity/{_appId}/?webUrl=https://localhost:44302/tab/{orderId}",
            new List<Microsoft.Graph.KeyValuePair> { new Microsoft.Graph.KeyValuePair { Name = "restaurant", Value = restaurant } });
    }

    public async Task SendActivityFeedTypeDeleted(string addresserId, string addresseeId, string restaurant)
    {
        await SendActivityNotification(addresseeId, "orderDeleted", "Your order has been deleted",
            $"https://teams.microsoft.com/l/entity/{_appId}/?webUrl=https://localhost:44302&label=tab",
            new List<Microsoft.Graph.KeyValuePair> { new Microsoft.Graph.KeyValuePair { Name = "restaurant", Value = restaurant } });
    }

    public async Task SendActivityFeedTypeDelivered(string addresserId, string addresseeId, int orderId)
    {
        await SendActivityNotification(addresseeId, "orderDelivered", "Your order is ready.",
            $"https://teams.microsoft.com/l/entity/{_appId}/?webUrl=https://localhost:44302/tab/{orderId}",
            previewText: new ItemBody { Content = "The Order has been delivered." });
    }

    private AadUserConversationMember CreateChatMember(string userId)
    {
        return new AadUserConversationMember
        {
            Roles = new List<string> { "owner" },
            AdditionalData = new Dictionary<string, object>
            {
                { "user@odata.bind", $"https://graph.microsoft.com/v1.0/users('{userId}')" }
            }
        };
    }

    private async Task SendActivityNotification(string userId, string activityType, string value, string webUrl, List<Microsoft.Graph.KeyValuePair> templateParameters = null, ItemBody previewText = null)
    {
        var topic = new TeamworkActivityTopic
        {
            Source = TeamworkActivityTopicSource.Text,
            Value = value,
            WebUrl = webUrl
        };

        await _graphServiceClient.Users[userId].Teamwork.SendActivityNotification(topic: topic, activityType: activityType, templateParameters: templateParameters, previewText: previewText).Request().PostAsync();
    }
}
