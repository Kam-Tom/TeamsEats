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
public class GroupOrderController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<GroupOrderHub> _hubContext;

    public GroupOrderController(IMediator mediator, IHubContext<GroupOrderHub> hubContext)
    {
        _mediator = mediator;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroupOrderLiteDTO>>> GetAll()
    {
        var groupOrders = await _mediator.Send(new GetGroupOrdersLiteQuery());
        return Ok(groupOrders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GroupOrderLiteDTO>> Get(int id)
    {
        var groupOrder = await _mediator.Send(new GetGroupOrderLiteQuery(id));
        return Ok(groupOrder);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateGroupOrderDTO groupOrder)
    {
        var id = await _mediator.Send(new CreateGroupOrderCommand(groupOrder));
        await _hubContext.Clients.All.SendAsync("GroupOrderCreated", id);
        return Ok();
    }

}