using MediatR;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class CreateOrderItemCommandHandler : IRequestHandler<CreateOrderItemCommand>
{
    readonly IOrderItemsRepository _orderItemsRepository;
    readonly IGroupOrderRepository _groupOrderRepository;
    readonly IGraphService _graphService;
    public CreateOrderItemCommandHandler(IOrderItemsRepository orderItemsRepository, IGroupOrderRepository groupOrderRepository, IGraphService graphService)
    {
        _orderItemsRepository = orderItemsRepository;
        _groupOrderRepository = groupOrderRepository;
        _graphService = graphService;
    }
    public async Task Handle(CreateOrderItemCommand request, CancellationToken cancellationToken)
    {
        var userID = await _graphService.GetUserID();
        var userDisplayName = await _graphService.GetUserDisplayName(userID);
        var groupOrder = await _groupOrderRepository.GetGroupOrderAsync(request.CreateOrderItemDTO.GroupOrderId);

        var orderItem = new OrderItem()
        {
            UserId = userID,
            UserDisplayName = userDisplayName,
            DishName = request.CreateOrderItemDTO.DishName,
            Price = request.CreateOrderItemDTO.Price,
            AdditionalInfo = request.CreateOrderItemDTO.AdditionalInfo,
            GroupOrder = groupOrder
        };

        await _orderItemsRepository.CreateOrderItemAsync(orderItem);

    }
}
