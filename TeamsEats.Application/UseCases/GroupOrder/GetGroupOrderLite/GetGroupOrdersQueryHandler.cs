using MediatR;
using TeamsEats.Application.DTOs;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class GetOrderSummaryQueryHandler : IRequestHandler<GetOrderSummaryQuery, OrderSummaryDTO>
{
    readonly IOrderRepository _orderRepository;
    readonly IGraphService _graphService;

    public GetOrderSummaryQueryHandler(IOrderRepository orderRepository, IGraphService graphService)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
    }

    public async Task<OrderSummaryDTO> Handle(GetOrderSummaryQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderAsync(request.Id);
        
        var userId = await _graphService.GetUserId();
        bool isOwner = order.AuthorId == userId;
        bool isParticipating = order.Items.Exists(g => g.AuthorId == userId);

        double orderCost = order.Items.Sum(o => o.Price);
        double ordersCount = order.Items.Count + 1;
        double deliveryCost = orderCost > order.MinimalPriceForFreeDelivery ? 0 : order.DeliveryFee / ordersCount;

        var authorPhoto = await _graphService.GetPhoto(order.AuthorId);

        var groupOrderDTO = new OrderSummaryDTO
        {
            Id = order.Id,
            IsOwner = isOwner,
            IsParticipating = isParticipating,
            AuthorName = order.AuthorName,
            AuthorPhoto = authorPhoto,
            Restaurant = order.Restaurant,
            DeliveryCost = deliveryCost,
            Status = order.Status,
            ClosingTime = order.ClosingTime
        };

        return groupOrderDTO;
    }
}
