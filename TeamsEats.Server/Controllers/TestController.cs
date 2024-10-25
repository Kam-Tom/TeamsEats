using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using TeamsEats.Application.UseCases;
using KeyValuePair = Microsoft.Graph.KeyValuePair;


namespace TeamsEats.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly GraphServiceClient _graphServiceClient;
        IMediator _mediator;
        private readonly string _adresserId = "605dc719-b09c-48b6-8aec-37e7e6dbbff0"; // Replace with actual adresser ID

        public TestController(GraphServiceClient graphServiceClient, IMediator mediator)
        {
            _graphServiceClient = graphServiceClient;
            _mediator = mediator;
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


        [HttpGet("TEST")]
        [Authorize]
        public async Task<IActionResult> SendNotification()
        {
            return Ok("TEST WORKS");
        }
    }
}
