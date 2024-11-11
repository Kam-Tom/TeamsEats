using MediatR;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IGraphService _graphService;

    public DeleteOrderCommandHandler(IOrderRepository orderRepository, IGraphService graphService)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
    }

    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var order = await _orderRepository.GetOrderAsync(request.OrderId);
        if (order.AuthorId != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to delete this order");
        }
        await _orderRepository.DeleteOrderAsync(order);

        if(order.Status == Domain.Enums.Status.Delivered)
        {
            return;
        }


        var addressees = order.Items.Select(o => o.AuthorId).Where(id => id != userId).Distinct();

        var feedTasks = new List<Task>();
        foreach (var addressee in addressees)
        {
            feedTasks.Add(_graphService.SendActivityFeedTypeDeleted(userId, addressee, order.Restaurant));
        }

        await Task.WhenAll(feedTasks);

    }
}
