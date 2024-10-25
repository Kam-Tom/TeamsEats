﻿using MediatR;
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
public class OrderItemController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<GroupOrderHub> _hubContext;


    public OrderItemController(IMediator mediator, IHubContext<GroupOrderHub> hubContext)
    {
        _mediator = mediator;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderItemDTO orderItem)
    {
        await _mediator.Send(new CreateOrderItemCommand(orderItem));
        await _hubContext.Clients.All.SendAsync("GroupOrderUpdated", orderItem.GroupOrderId);
        return Ok();
    }

    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] UpdateOrderItemDTO orderItem)
    {
        await _mediator.Send(new UpdateOrderItemCommand(orderItem));
        await _hubContext.Clients.All.SendAsync("GroupOrderUpdated", orderItem.GroupOrderId);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteOrderItemCommand(id));
        return Ok();
    }

    [HttpPost("SendComment")]
    public async Task<IActionResult> CommentOrderItem([FromBody] CommentOrderItemDTO commentOrderItemDTO)
    {
        await _mediator.Send(new CommentOrderItemCommand(commentOrderItemDTO));
        return Ok();
    }
}