using MediatR;
using TeamsEats.Application.DTOs;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class GetOrderSummariesQueryHandler : IRequestHandler<GetOrderSummariesQuery, IEnumerable<OrderSummaryDTO>>
{
    readonly IOrderRepository _orderRepository;
    readonly IGraphService _graphService;

    public GetOrderSummariesQueryHandler(IOrderRepository orderRepository, IGraphService graphService)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
    }

    public async Task<IEnumerable<OrderSummaryDTO>> Handle(GetOrderSummariesQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetOrdersAsync();

        var orderDTOs = new List<OrderSummaryDTO>();
        var photoTasks = new List<Task<string>>();

        foreach (var order in orders)
        {
            var userId = await _graphService.GetUserId();
            bool isOwner = order.AuthorId == userId;
            bool isParticipating = order.Items.Exists(g => g.AuthorId == userId);

            double orderCost = order.Items.Sum(o => o.Price);
            double ordersCount = order.Items.Count + 1;
            double deliveryCost = orderCost > order.MinimalPriceForFreeDelivery ? 0 : order.DeliveryFee / ordersCount;


            var photoTask = _graphService.GetPhoto(order.AuthorId);
            photoTasks.Add(photoTask);

            var orderDTO = new OrderSummaryDTO
            {
                Id = order.Id,
                IsOwner = isOwner,
                IsParticipating = isParticipating,
                AuthorName = order.AuthorName,
                Restaurant = order.Restaurant,
                DeliveryCost = deliveryCost,
                Status = order.Status,
                ClosingTime = order.ClosingTime,
            };

            orderDTOs.Add(orderDTO);
        }

        var photos = await Task.WhenAll(photoTasks);

        for (int i = 0; i < orderDTOs.Count; i++)
        {
            orderDTOs[i].AuthorPhoto = photos[i];
        }

        return orderDTOs;
    }
}
