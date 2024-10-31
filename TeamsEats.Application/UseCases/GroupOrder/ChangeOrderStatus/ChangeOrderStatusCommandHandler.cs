using MediatR;
using TeamsEats.Domain.Enums;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand>
{
    readonly IOrderRepository _orderRepository;
    readonly IGraphService _graphService;
    public ChangeOrderStatusCommandHandler(IOrderRepository orderRepository, IGraphService graphService)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
    }

    public async Task Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderAsync(request.OrderId);
        var userId = request.UserId;
        var itemsSum = order.Items.Sum(i => i.Price);

        if (order.AuthorId != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to change the status of this order");
        }
        if(order.MinimalPrice > itemsSum)
        {
            throw new InvalidOperationException("Minimal price was not reached");
        }
        if(request.Status < order.Status)
        {
            throw new InvalidOperationException("You are not allowed to downgrade the order status");
        }
        order.Status = request.Status;

        await _orderRepository.UpdateOrderAsync(order);

        var users = order.Items.Select(i => i.AuthorId).Distinct();
        var tasks = new List<Task>();
        foreach (var user in users)
        {
            if (user == order.AuthorId)
                continue;

            if(order.Status == Status.Ordered)
            {
                tasks.Add(_graphService.SendActivityFeedTypeOrdered(order.AuthorId, user, order.Restaurant, order.Id));
            }
            else if(order.Status == Status.Delivered)
            {
                tasks.Add(_graphService.SendActivityFeedTypeDelivered(order.AuthorId, user, order.Id));
            }
        }
        await Task.WhenAll(tasks);



    }
}
