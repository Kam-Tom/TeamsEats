using MediatR;
using TeamsEats.Application.DTOs;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class GetGroupOrdersLiteQueryHandler : IRequestHandler<GetGroupOrdersLiteQuery, IEnumerable<GroupOrderLiteDTO>>
{
    readonly IGroupOrderRepository _groupOrderRepository;
    readonly IGraphService _graphService;

    public GetGroupOrdersLiteQueryHandler(IGroupOrderRepository groupOrderRepository, IGraphService graphService)
    {
        _groupOrderRepository = groupOrderRepository;
        _graphService = graphService;
    }

    public async Task<IEnumerable<GroupOrderLiteDTO>> Handle(GetGroupOrdersLiteQuery request, CancellationToken cancellationToken)
    {
        var groupOrders = await _groupOrderRepository.GetGroupsOrdersAsync();

        var groupOrdersDTO = new List<GroupOrderLiteDTO>();
        var photoTasks = new List<Task<string>>();

        foreach (var groupOrder in groupOrders)
        {
            var userId = await _graphService.GetUserID();
            bool isOwnedByUser = groupOrder.UserId == userId;
            bool hasItemInOrder = groupOrder.OrderItems.Exists(g => g.UserId == userId);

            double orderCost = groupOrder.OrderItems.Sum(o => o.Price);
            double ordersCount = groupOrder.OrderItems.Count + 1;
            double deliveryCost = deliveryCost = orderCost > groupOrder.MinimalPriceForFreeDelivery ? 0 : groupOrder.DeliveryFee / ordersCount;


            var photoTask = _graphService.GetPhoto(groupOrder.UserId);
            photoTasks.Add(photoTask);

            var groupOrderDTO = new GroupOrderLiteDTO
            {
                Id = groupOrder.Id,
                IsOwnedByUser = isOwnedByUser,
                HasItemInOrder = hasItemInOrder,
                AuthorName = groupOrder.UserDisplayName,
                Restaurant = groupOrder.RestaurantName,
                DeliveryCost = deliveryCost,
                Status = groupOrder.Status,
                ClosingTime = groupOrder.ClosingTime,
            };

            groupOrdersDTO.Add(groupOrderDTO);
        }

        var photos = await Task.WhenAll(photoTasks);

        for (int i = 0; i < groupOrdersDTO.Count; i++)
        {
            groupOrdersDTO[i].AuthorPhoto = photos[i];
        }

        return groupOrdersDTO;
    }
}
