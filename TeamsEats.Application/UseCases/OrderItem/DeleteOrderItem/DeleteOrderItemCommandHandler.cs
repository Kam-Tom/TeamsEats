using MediatR;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class DeleteOrderItemCommandHandler: IRequestHandler<DeleteOrderItemCommand>
{
    readonly IOrderItemsRepository _orderItemsRepository;
    readonly IGraphService _graphService;
    public DeleteOrderItemCommandHandler(IOrderItemsRepository orderItemsRepository, IGraphService graphService)
    {
        _orderItemsRepository = orderItemsRepository;
        _graphService = graphService;
    }

    public async Task Handle(DeleteOrderItemCommand request, CancellationToken cancellationToken)
    {
        var userId = _graphService.GetUserID().Result;
        var deleteOrderItemID = request.OrderItemId;
        var orderItem = _orderItemsRepository.GetOrderItemAsync(deleteOrderItemID).Result;

        if (orderItem.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to delete this order item");
        }
        if (orderItem.GroupOrder.Status != Domain.Enums.GroupOrderStatus.Open)
        {
            throw new InvalidOperationException("You are not allowed to delete this order item");
        }

        await _orderItemsRepository.DeleteOrderItemAsync(orderItem);

    }

}
