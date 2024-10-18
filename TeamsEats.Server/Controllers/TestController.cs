using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using KeyValuePair = Microsoft.Graph.KeyValuePair;


namespace TeamsEats.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly GraphServiceClient _graphServiceClient;
        private readonly string _adresserId = "605dc719-b09c-48b6-8aec-37e7e6dbbff0"; // Replace with actual adresser ID

        public TestController(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        [HttpGet(Name = "GetTest")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                var photoStream = await _graphServiceClient.Me.Photo.Content.Request().GetAsync();
                using (var memoryStream = new MemoryStream())
                {
                    await photoStream.CopyToAsync(memoryStream);
                    var photoBytes = memoryStream.ToArray();
                    var base64Photo = Convert.ToBase64String(photoBytes);
                    return Ok(new { photo = base64Photo });
                }
            }
            catch (ServiceException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }

        [HttpPost("SendMessage")]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromBody] string message)
        {
            try
            {
                string userId = _graphServiceClient.Me.Request().GetAsync().Result.Id;
                var members = new List<ConversationMember>
                {
                    new AadUserConversationMember
                    {
                        Roles = new List<string> { "owner" },
                        AdditionalData = new Dictionary<string, object>
                        {
                            { "user@odata.bind", $"https://graph.microsoft.com/v1.0/users('{userId}')" }
                        }
                    },
                    new AadUserConversationMember
                    {
                        Roles = new List<string> { "owner" },
                        AdditionalData = new Dictionary<string, object>
                        {
                            { "user@odata.bind", $"https://graph.microsoft.com/v1.0/users('{_adresserId}')" }
                        }
                    }
                };

                var m = new ChatMembersCollectionPage();
                foreach (var mm in members)
                {
                    m.Add(mm);
                }
                var requestBody = new Chat
                {
                    ChatType = ChatType.OneOnOne,
                    Members = m
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

                return Ok("Message sent successfully.");
            }
            catch (ServiceException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }

        [HttpPost("SendNotification")]
        [Authorize]
        public async Task<IActionResult> SendNotification()
        {
            try
            {
                string userId = _graphServiceClient.Me.Request().GetAsync().Result.Id;

                var appId = "7e0988e5-128c-4a09-a6d9-036685c900b0";

                var Topic = new TeamworkActivityTopic
                {
                    Source = TeamworkActivityTopicSource.Text,
                    Value = "activity - topic",
                    WebUrl = $"https://teams.microsoft.com/l/entity/{appId}/?webUrl=https://localhost:44302&label=tab",
                };
                var ActivityType = "taskCreated";
                var PreviewText = new ItemBody
                {
                    Content = "New Task Created",
                };
                var TemplateParameters = new List<KeyValuePair>
                    {
                        new KeyValuePair
                        {
                            Name = "workItemId",
                            Value = "created-value",
                        },
                    };


                await _graphServiceClient.Users[_adresserId].Teamwork.SendActivityNotification(topic:Topic,activityType:ActivityType,previewText:PreviewText,templateParameters:TemplateParameters).Request().PostAsync();

                return Ok("Notification sent successfully.");
            }
            catch (ServiceException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
        }
    }
}
