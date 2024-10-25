using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamsEats.Domain.Enums;
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
            using (var memoryStream = new MemoryStream())
            {
                await photoStream.CopyToAsync(memoryStream);
                var photoBytes = memoryStream.ToArray();
                var base64Photo = Convert.ToBase64String(photoBytes);
                return base64Photo;
            }
        }
        catch(ServiceException ex)
        {
            if(ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                return "";
            else
                throw;
        }

    }

    public async Task<string> GetUserID()
    {
        var user = await _graphServiceClient.Me.Request().GetAsync();
        var id = user.Id;
        return id;
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
            new AadUserConversationMember
            {
                Roles = new List<string> { "owner" },
                AdditionalData = new Dictionary<string, object>
                {
                    { "user@odata.bind", $"https://graph.microsoft.com/v1.0/users('{addresserId}')" }
                }
            },
            new AadUserConversationMember
            {
                Roles = new List<string> { "owner" },
                AdditionalData = new Dictionary<string, object>
                {
                    { "user@odata.bind", $"https://graph.microsoft.com/v1.0/users('{addresseeId}')" }
                }
            }
        };

        var requestBody = new Chat
        {
            ChatType = ChatType.OneOnOne,
            Members = members
        };

        var chat = await _graphServiceClient.Chats.Request().AddAsync(requestBody);

            var chatMessage = new ChatMessage
            {
                Body = new ItemBody
                {
                    Content = message
                }
            };

        await _graphServiceClient.Chats[chat.Id].Messages.Request().AddAsync(chatMessage);
        
    }

    public async Task SendActivityFeedTypeClosed(string addresserId, string addresseeId, string restaurant, int groupOrderId)
    {
        var Topic = new TeamworkActivityTopic
        {
            Source = TeamworkActivityTopicSource.Text,
            Value = "Everyone is waiting for your payment.",
            WebUrl = $"https://teams.microsoft.com/l/entity/{_appId}/?webUrl=https://localhost:44302/tab/{groupOrderId}",
        };
        var ActivityType = "groupOrderClosed";

        var TemplateParameters = new List<Microsoft.Graph.KeyValuePair>
        {
            new Microsoft.Graph.KeyValuePair
            {
                Name = "resteurant",
                Value = restaurant,
            }
        };

        await _graphServiceClient.Users[addresseeId].Teamwork.SendActivityNotification(topic: Topic, activityType: ActivityType, templateParameters: TemplateParameters).Request().PostAsync();
    }
    public async Task SendActivityFeedTypeDeleted(string addresserId, string addresseeId, string restaurant)
    {
        var Topic = new TeamworkActivityTopic
        {
            Source = TeamworkActivityTopicSource.Text,
            Value = "Your order has been deleted",
            WebUrl = $"https://teams.microsoft.com/l/entity/{_appId}/?webUrl=https://localhost:44302&label=tab",
        };
        var ActivityType = "groupOrderDeleted";
        var TemplateParameters = new List<Microsoft.Graph.KeyValuePair>
        {
            new Microsoft.Graph.KeyValuePair
            {
                Name = "resteurant",
                Value = restaurant,
            },
        };

        await _graphServiceClient.Users[addresseeId].Teamwork.SendActivityNotification(topic: Topic, activityType: ActivityType, templateParameters: TemplateParameters).Request().PostAsync();
    }
    public async Task SendActivityFeedTypeDelivered(string addresserId, string addresseeId, int groupOrderId)
    {
        var Topic = new TeamworkActivityTopic
        {
            Source = TeamworkActivityTopicSource.Text,
            Value = "Your order is ready.",
            WebUrl = $"https://teams.microsoft.com/l/entity/{_appId}/?webUrl=https://localhost:44302/tab/{groupOrderId}",
        };
        var ActivityType = "groupOrderDelivered";
        var PreviewText = new ItemBody
        {
            Content = "The Order has been delivered.",
        };
 

        await _graphServiceClient.Users[addresseeId].Teamwork.SendActivityNotification(topic: Topic, activityType: ActivityType, previewText: PreviewText).Request().PostAsync();
    }

}
