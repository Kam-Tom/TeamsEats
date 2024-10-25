using MediatR;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class CommentOrderItemCommandHandler : IRequestHandler<CommentOrderItemCommand>
{
    readonly IOrderItemsRepository _orderItemsRepository;
    readonly IGraphService _graphService;
    public CommentOrderItemCommandHandler(IOrderItemsRepository orderItemsRepository, IGroupOrderRepository groupOrderRepository, IGraphService graphService)
    {
        _orderItemsRepository = orderItemsRepository;
        _graphService = graphService;
    }
    public async Task Handle(CommentOrderItemCommand request, CancellationToken cancellationToken)
    {
        var orderItem = await _orderItemsRepository.GetOrderItemAsync(request.CommentOrderItemDTO.OrderItemID);
        var message = request.CommentOrderItemDTO.Message;
        
        var addresserId = await _graphService.GetUserID();
        var addresseeId = orderItem.UserId;

        if(orderItem.GroupOrder.UserId != addresserId)
        {
            throw new UnauthorizedAccessException("You are not allowed to comment this order item");
        }
        if(orderItem.UserId == addresserId)
        {
            throw new UnauthorizedAccessException("You are not allowed to comment your own order item");
        }

        await _graphService.SendMessage(addresserId, addresseeId, message);

    }
}
