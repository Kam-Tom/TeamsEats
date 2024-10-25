using MediatR;
using TeamsEats.Domain.Enums;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class DeleteGroupOrderCommandHandler : IRequestHandler<DeleteGroupOrderCommand>
{
    private readonly IGroupOrderRepository _groupOrderRepository;
    private readonly IGraphService _graphService;

    public DeleteGroupOrderCommandHandler(
        IGroupOrderRepository groupOrderRepository,
        IGraphService graphService)
    {
        _groupOrderRepository = groupOrderRepository;
        _graphService = graphService;
    }

    public async Task Handle(DeleteGroupOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = await _graphService.GetUserID();
        var groupOrder = await _groupOrderRepository.GetGroupOrderAsync(request.GroupOrderId);
        if (groupOrder.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to delete this group order");
        }
        await _groupOrderRepository.DeleteGroupOrderAsync(groupOrder);

        var addressees = groupOrder.OrderItems.Select(o => o.UserId).Where(id => id != userId);

        foreach (var addressee in addressees)
        {
            await _graphService.SendActivityFeedTypeDeleted(userId, addressee, groupOrder.RestaurantName);
        }

    }
}
