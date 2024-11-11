using MediatR;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand>
{
    readonly IOrderRepository _orderRepository;
    readonly IGraphService _graphService;
    public UpdateItemCommandHandler(IOrderRepository orderRepository, IGraphService graphService)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
    }
    public async Task Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {

        var userID = request.UserId;
        var updatedItem = await _orderRepository.GetItemAsync(request.ItemId);
        var order = updatedItem.Order;
        if (updatedItem.AuthorId != userID)
        {
            throw new UnauthorizedAccessException("You are not allowed to update this item");
        }
        if(order.Status != Domain.Enums.Status.Open)
        {
            throw new InvalidOperationException("You are not allowed to update this item");
        }

        order.CurrentPrice -= updatedItem.Price;

        updatedItem.Price = request.UpdateItemDTO.Price;
        updatedItem.AdditionalInfo = request.UpdateItemDTO.AdditionalInfo;
        updatedItem.Dish = request.UpdateItemDTO.Dish;

        order.CurrentPrice += updatedItem.Price;

        var usersItems = order.Items.GroupBy(i => i.AuthorId).ToDictionary(g => g.Key, g => g.Count());

        var currentDeliveryFee = order.CurrentPrice >= order.MinimalPriceForFreeDelivery ? 0 : order.DeliveryFee / Math.Max(usersItems.Count, 1);
        order.CurrentDeliveryFee = currentDeliveryFee;

        await _orderRepository.UpdateOrderAsync(order);

    }
}
