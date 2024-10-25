using MediatR;
using TeamsEats.Application.DTOs;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class GetGroupOrderLiteQueryHandler : IRequestHandler<GetGroupOrderLiteQuery, GroupOrderLiteDTO>
{
    readonly IGroupOrderRepository _groupOrderRepository;
    readonly IGraphService _graphService;

    public GetGroupOrderLiteQueryHandler(IGroupOrderRepository groupOrderRepository, IGraphService graphService)
    {
        _groupOrderRepository = groupOrderRepository;
        _graphService = graphService;
    }

    public async Task<GroupOrderLiteDTO> Handle(GetGroupOrderLiteQuery request, CancellationToken cancellationToken)
    {
        var groupOrder = await _groupOrderRepository.GetGroupOrderAsync(request.GroupOrderId);
        
        var userId = await _graphService.GetUserID();
        bool isOwnedByUser = groupOrder.UserId == userId;
        bool hasItemInOrder = groupOrder.OrderItems.Exists(g => g.UserId == userId);

        double orderCost = groupOrder.OrderItems.Sum(o => o.Price);
        double ordersCount = groupOrder.OrderItems.Count + 1;
        double deliveryCost = orderCost > groupOrder.MinimalPriceForFreeDelivery ? 0 : groupOrder.DeliveryFee / ordersCount;

        var authorPhoto = await _graphService.GetPhoto(groupOrder.UserId);

        var groupOrderDTO = new GroupOrderLiteDTO
        {
            Id = groupOrder.Id,
            IsOwnedByUser = isOwnedByUser,
            HasItemInOrder = hasItemInOrder,
            AuthorName = groupOrder.UserDisplayName,
            AuthorPhoto = authorPhoto,
            Restaurant = groupOrder.RestaurantName,
            DeliveryCost = deliveryCost,
            Status = groupOrder.Status,
            ClosingTime = groupOrder.ClosingTime
        };

        return groupOrderDTO;
    }
}
