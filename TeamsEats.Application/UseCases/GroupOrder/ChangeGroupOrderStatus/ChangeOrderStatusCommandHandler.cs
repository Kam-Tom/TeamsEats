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
        var order = await _orderRepository.GetOrderAsync(request.ChangeOrderStatusDTO.Id);
        var userId = await _graphService.GetUserId();
        if(order.AuthorId != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to change the status of this order");
        }
        order.Status = request.ChangeOrderStatusDTO.Status;

        await _orderRepository.UpdateOrderAsync(order);

        var addressees = order.Items.Select(o => o.AuthorId).Where(id => id != userId);

        var feedTasks = new List<Task>();

        foreach (var addressee in addressees)
        {
            if (order.Status == Status.Delivered)
                feedTasks.Add(_graphService.SendActivityFeedTypeDelivered(userId, addressee, order.Id));
            else if (order.Status == Status.Closed)
                feedTasks.Add(_graphService.SendActivityFeedTypeClosed(userId, addressee, order.Restaurant, order.Id));
        }   

        await Task.WhenAll(feedTasks);
    }
}
