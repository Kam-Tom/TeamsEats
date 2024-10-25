using MediatR;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class UpdateOrderItemCommandHandler : IRequestHandler<UpdateOrderItemCommand>
{
    readonly IOrderItemsRepository _orderItemsRepository;
    readonly IGraphService _graphService;
    public UpdateOrderItemCommandHandler(IOrderItemsRepository orderItemsRepository, IGraphService graphService)
    {
        _orderItemsRepository = orderItemsRepository;
        _graphService = graphService;
    }
    public async Task Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
    {

        var userID = await _graphService.GetUserID();
        var orderItem = await _orderItemsRepository.GetOrderItemAsync(request.UpdateOrderItemDTO.OrderItemID);

        if (orderItem.UserId != userID)
        {
            throw new UnauthorizedAccessException("You are not allowed to update this order item");
        }
        if(orderItem.GroupOrder.Status != Domain.Enums.GroupOrderStatus.Open)
        {
            throw new InvalidOperationException("You are not allowed to update this order item");
        }

        orderItem.Price = request.UpdateOrderItemDTO.Price;
        orderItem.AdditionalInfo = request.UpdateOrderItemDTO.AdditionalInfo;
        orderItem.DishName = request.UpdateOrderItemDTO.DishName;

        await _orderItemsRepository.UpdateOrderItemAsync(orderItem);

        //await _graphService.SendActivityFeed();

    }
}
