using MediatR;
using TeamsEats.Application.DTOs;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class OrderItemQueryHandler: IRequestHandler<OrderItemQuery, OrderItemDTO>
{
    readonly IOrderItemsRepository _orderItemsRepository;
    readonly IGraphService _graphService;
    public OrderItemQueryHandler(IOrderItemsRepository orderItemsRepository, IGraphService graphService)
    {
        _orderItemsRepository = orderItemsRepository;
        _graphService = graphService;
    }

    public async Task<OrderItemDTO> Handle(OrderItemQuery request, CancellationToken cancellationToken)
    {
        var userId = await _graphService.GetUserID();
        var orderItem = await _orderItemsRepository.GetOrderItemAsync(request.id);

        if (orderItem.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to delete this order item");
        }
        if (orderItem.GroupOrder.Status != Domain.Enums.GroupOrderStatus.Open)
        {
            throw new InvalidOperationException("You are not allowed to delete this order item");
        }

        return new OrderItemDTO()
        {
            Id = orderItem.Id,
            AuthorName = orderItem.UserDisplayName,
            DishName = orderItem.DishName,
            Price = orderItem.Price,
            IsOwner = orderItem.UserId == userId,
            GroupOrderId = orderItem.GroupOrderId,
            AdditionalInfo = orderItem.AdditionalInfo
        };

    }

}
