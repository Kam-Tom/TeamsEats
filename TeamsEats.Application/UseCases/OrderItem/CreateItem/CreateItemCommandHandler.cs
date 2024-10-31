using AutoMapper;
using MediatR;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand>
{
    readonly IOrderRepository _orderRepository;
    readonly IGraphService _graphService;
    readonly IMapper _mapper;
    public CreateItemCommandHandler(IOrderRepository orderRepository, IGraphService graphService, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
        _mapper = mapper;
    }
    public async Task Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var userID = request.UserId;
        var userDisplayName = await _graphService.GetUserDisplayName(userID);

        var order = await _orderRepository.GetOrderAsync(request.CreateItemDTO.OrderId);

        if (order.Status != Domain.Enums.Status.Open)
        {
            throw new InvalidOperationException("You are not allowed to add items not open order");
        }

        var newItem = _mapper.Map<Item>(request.CreateItemDTO);
        newItem.AuthorId = userID;
        newItem.AuthorName = userDisplayName;
        newItem.OrderId = request.CreateItemDTO.OrderId;
        
        order.Items.Add(newItem);
        order.CurrentPrice += newItem.Price;

        var usersItems = order.Items.GroupBy(i => i.AuthorId).ToDictionary(g => g.Key, g => g.Count());

        var currentDeliveryFee = order.CurrentPrice >= order.MinimalPriceForFreeDelivery ? 0 : order.DeliveryFee / Math.Max(usersItems.Count, 1);
        order.CurrentDeliveryFee = currentDeliveryFee;

        await _orderRepository.UpdateOrderAsync(order);
    }

}
