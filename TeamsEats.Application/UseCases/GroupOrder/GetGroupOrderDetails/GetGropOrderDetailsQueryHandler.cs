using MediatR;
using TeamsEats.Application.DTOs;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class GetGroupOrderDetailsQueryHandler : IRequestHandler<GetGroupOrderDetailsQuery, GroupOrderDetailsDTO>
{
    readonly IGroupOrderRepository _groupOrderRepository;
    readonly IGraphService _graphService;
    public GetGroupOrderDetailsQueryHandler(IGroupOrderRepository groupOrderRepository, IGraphService graphService)
    {
        _groupOrderRepository = groupOrderRepository;
        _graphService = graphService;
    }

    public async Task<GroupOrderDetailsDTO> Handle(GetGroupOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var groupOrder = await _groupOrderRepository.GetGroupOrderAsync(request.GroupOrderId);
        var userId = await _graphService.GetUserID();
        var userPhotoUrl = await _graphService.GetPhoto(groupOrder.UserId);
        var orderItemsDTO = new List<OrderItemDTO>();

        double orderCost = groupOrder.OrderItems.Sum(o => o.Price);
        double ordersCount = groupOrder.OrderItems.Count + 1;
        double deliveryCost = orderCost > groupOrder.MinimalPriceForFreeDelivery ? 0 : groupOrder.DeliveryFee / ordersCount;


        foreach (var orderItem in groupOrder.OrderItems)
        {
            var clientPhotoUrl = await _graphService.GetPhoto(orderItem.UserId);
            var orderItemDto = new OrderItemDTO
            {
                Id = orderItem.Id,
                IsOwner = orderItem.UserId == userId,
                DishName = orderItem.DishName,
                AuthorName = orderItem.UserDisplayName,
                AuthorPhoto = clientPhotoUrl,
                Price = orderItem.Price,
                AdditionalInfo = orderItem.AdditionalInfo
            };

            orderItemsDTO.Add(orderItemDto);
        }

        var groupOrderDTO = new GroupOrderDetailsDTO()
        {
            Id = groupOrder.Id,
            isOwnedByUser = groupOrder.UserId == userId,
            AuthorName = groupOrder.UserDisplayName,
            AuthorPhoto = userPhotoUrl,
            PhoneNumber = groupOrder.PhoneNumber,
            Restaurant = groupOrder.RestaurantName,
            BankAccount = groupOrder.BankAccount,
            MinimalPrice = groupOrder.MinimalPrice,
            DeliveryCost = deliveryCost,
            MinimalPriceForFreeDelivery = groupOrder.MinimalPriceForFreeDelivery,
            OrderItems = orderItemsDTO,
            Status = groupOrder.Status,
            ClosingTime = groupOrder.ClosingTime
        };

        return groupOrderDTO;
    }
}
