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
public class GroupOrderDetailsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<GroupOrderHub> _hubContext;

    public GroupOrderDetailsController(IMediator mediator, IHubContext<GroupOrderHub> hubContext)
    {
        _mediator = mediator;
        _hubContext = hubContext;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GroupOrderDetailsDTO>> Get(int id)
    {
        var groupOrder = await _mediator.Send(new GetGroupOrderDetailsQuery(id));
        return Ok(groupOrder);
    }
    [HttpPatch]
    public async Task<ActionResult> Update([FromBody] ChangeGroupOrderStatusDTO groupOrder)
    {
        await _mediator.Send(new ChangeGroupOrdersStatusCommand(groupOrder));
        await _hubContext.Clients.All.SendAsync("GroupOrderUpdated", groupOrder.GroupOrderID);
        return Ok();
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteGroupOrderCommand(id));
        await _hubContext.Clients.All.SendAsync("GroupOrderDeleted", id);
        return Ok();
    }
}