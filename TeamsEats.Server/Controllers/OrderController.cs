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
        var orderSummaries = await _mediator.Send(new GetOrderSummariesQuery());
        return Ok(orderSummaries);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderSummaryDTO>> GetSummary(int id)
    {
        var orderSummary = await _mediator.Send(new GetOrderSummaryQuery(id));
        return Ok(orderSummary);
    }

    [HttpGet("{id}/detail")]
    public async Task<ActionResult<OrderDetailsDTO>> GetDetail(int id)
    {
        var orderDetails = await _mediator.Send(new GetOrderDetailQuery(id));
        return Ok(orderDetails);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateOrderDTO order)
    {
        var id = await _mediator.Send(new CreateOrderCommand(order));
        await _hubContext.Clients.All.SendAsync("OrderCreated", id);
        return Created();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] ChangeOrderStatusDTO order)
    {
        order.Id = id;
        await _mediator.Send(new ChangeOrderStatusCommand(order));
        await _hubContext.Clients.All.SendAsync("OrderUpdated", order.Id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteOrderCommand(id));
        await _hubContext.Clients.All.SendAsync("OrderDeleted", id);
        return NoContent();
    }
}
