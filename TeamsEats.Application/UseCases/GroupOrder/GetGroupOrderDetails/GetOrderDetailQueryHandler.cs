using MediatR;
using TeamsEats.Application.DTOs;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class GetGroupOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailQuery, OrderDetailsDTO>
{
    readonly IOrderRepository _orderRepository;
    readonly IGraphService _graphService;
    public GetGroupOrderDetailsQueryHandler(IOrderRepository orderRepository, IGraphService graphService)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
    }

    public async Task<OrderDetailsDTO> Handle(GetOrderDetailQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderAsync(request.Id);
        var userId = await _graphService.GetUserId();
        var userPhoto = await _graphService.GetPhoto(order.AuthorId);
        var itemDTOs = new List<ItemDTO>();

        double orderCost = order.Items.Sum(o => o.Price);
        double ordersCount = Math.Max(order.Items.Count,1);
        double deliveryCost = orderCost > order.MinimalPriceForFreeDelivery ? 0 : order.DeliveryFee / ordersCount;


        foreach (var item in order.Items)
        {
            var photo = await _graphService.GetPhoto(item.AuthorId);
            var orderItemDto = new ItemDTO
            {
                Id = item.Id,
                IsOwner = item.AuthorId == userId,
                Dish = item.Dish,
                AuthorName = item.AuthorName,
                AuthorPhoto = photo,
                Price = item.Price,
                AdditionalInfo = item.AdditionalInfo
            };

            itemDTOs.Add(orderItemDto);
        }

        var groupOrderDTO = new OrderDetailsDTO()
        {
            Id = order.Id,
            IsOwner = order.AuthorId == userId,
            AuthorName = order.AuthorName,
            AuthorPhoto = userPhoto,
            PhoneNumber = order.PhoneNumber,
            Restaurant = order.Restaurant,
            BankAccount = order.BankAccount,
            MinimalPrice = order.MinimalPrice,
            DeliveryCost = deliveryCost,
            MinimalPriceForFreeDelivery = order.MinimalPriceForFreeDelivery,
            Items = itemDTOs,
            Status = order.Status,
            ClosingTime = order.ClosingTime
        };

        return groupOrderDTO;
    }
}
