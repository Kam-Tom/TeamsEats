using MediatR;
using TeamsEats.Domain.Enums;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class DeleteItemCommandHandler: IRequestHandler<DeleteItemCommand>
{
    readonly IOrderRepository _orderRepository;
    readonly IGraphService _graphService;
    public DeleteItemCommandHandler(IOrderRepository orderRepository, IGraphService graphService)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
    }

    public async Task Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var deletedItem = await _orderRepository.GetItemAsync(request.ItemId);
        var order = deletedItem.Order;

        if (deletedItem.AuthorId != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to delete this item");
        }
        if (order.Status != Status.Open)
        {
            throw new InvalidOperationException("You are not allowed to delete this item");
        }

        order.Items.Remove(deletedItem);
        order.CurrentPrice -= deletedItem.Price;

        var usersItems = order.Items.GroupBy(i => i.AuthorId).ToDictionary(g => g.Key, g => g.Count());
        var currentDeliveryFee = order.CurrentPrice >= order.MinimalPriceForFreeDelivery ? 0 : order.DeliveryFee / Math.Max(usersItems.Count, 1);
        order.CurrentDeliveryFee = currentDeliveryFee;

        await _orderRepository.UpdateOrderAsync(order);



        if(order.Status == Status.Delivered)
        {
            return;
        }

        var users = order.Items.Select(i => i.AuthorId).Distinct();
        var tasks = new List<Task>();
        foreach (var user in users)
        {
            tasks.Add(_graphService.SendActivityFeedTypeDeleted(order.AuthorId, user, order.Restaurant));
        }
        await Task.WhenAll(tasks);

    }

}
