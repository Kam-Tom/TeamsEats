using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TeamsEats.Application.DTOs;
using TeamsEats.Application.UseCases;
using TeamsEats.Domain.Enums;
using TeamsEats.Server.Hubs;

namespace TeamsEats.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<OrderHub> _hubContext;

    public OrderController(IMediator mediator, IHubContext<OrderHub> hubContext)
    {
        _mediator = mediator;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderSummaryDTO>>> GetAll()
    {
        var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")!.Value;
        var orderSummaries = await _mediator.Send(new GetOrderSummariesQuery(userId));
        return Ok(orderSummaries);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderSummaryDTO>> GetSummary(int id)
    {
        var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")!.Value;
        var orderSummary = await _mediator.Send(new GetOrderSummaryQuery(id, userId));
        return Ok(orderSummary);
    }

    [HttpGet("{id}/detail")]
    public async Task<ActionResult<OrderDetailsDTO>> GetDetail(int id)
    {
        var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")!.Value;
        var orderDetails = await _mediator.Send(new GetOrderDetailQuery(id, userId));
        return Ok(orderDetails);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateOrderDTO order)
    {
        var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")!.Value;
        var id = await _mediator.Send(new CreateOrderCommand(order, userId));
        await _hubContext.Clients.All.SendAsync("OrderCreated", id);
        return Created();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] Status status)
    {
        var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")!.Value;
        await _mediator.Send(new ChangeOrderStatusCommand(status, id, userId));
        await _hubContext.Clients.All.SendAsync("OrderUpdated", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")!.Value;
        await _mediator.Send(new DeleteOrderCommand(id, userId));
        await _hubContext.Clients.All.SendAsync("OrderDeleted", id);
        return NoContent();
    }
}
