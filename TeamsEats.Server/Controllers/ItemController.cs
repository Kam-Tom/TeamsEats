using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TeamsEats.Application.DTOs;
using TeamsEats.Application.UseCases;
using TeamsEats.Server.Hubs;
namespace TeamsEats.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<OrderHub> _hubContext;

    public ItemController(IMediator mediator, IHubContext<OrderHub> hubContext)
    {
        _mediator = mediator;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateItemDTO item)
    {
        var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")!.Value;
        await _mediator.Send(new CreateItemCommand(item, userId));
        await _hubContext.Clients.All.SendAsync("OrderUpdated", item.OrderId);
        return Created();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateItemDTO item)
    {
        var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")!.Value;
        await _mediator.Send(new UpdateItemCommand(item, id, userId));
        await _hubContext.Clients.All.SendAsync("OrderUpdated", item.OrderId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")!.Value;
        var item = await _mediator.Send(new ItemQuery(id));
        await _mediator.Send(new DeleteItemCommand(id, userId));
        await _hubContext.Clients.All.SendAsync("OrderUpdated", item.OrderId);
        return NoContent();
    }

    [HttpPost("{id}/comments")]
    public async Task<ActionResult> CommentOrderItem([FromRoute] int id, [FromBody] string Message)
    {
        var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")!.Value;
        await _mediator.Send(new CommentItemCommand(Message, id, userId));
        return NoContent();
    }
}
