using MediatR;
using TeamsEats.Domain.Enums;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class ChangeGroupOrdersStatusCommandHandler : IRequestHandler<ChangeGroupOrdersStatusCommand>
{
    readonly IGroupOrderRepository _groupOrderRepository;
    readonly IGraphService _graphService;
    public ChangeGroupOrdersStatusCommandHandler(IGroupOrderRepository groupOrderRepository, IGraphService graphService)
    {
        _groupOrderRepository = groupOrderRepository;
        _graphService = graphService;
    }

    public async Task Handle(ChangeGroupOrdersStatusCommand request, CancellationToken cancellationToken)
    {
        var groupOrder = await _groupOrderRepository.GetGroupOrderAsync(request.dto.GroupOrderID);
        var userId = await _graphService.GetUserID();
        if(groupOrder.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to change the status of this group order");
        }
        groupOrder.Status = request.dto.Status;

        await _groupOrderRepository.UpdateGroupOrderAsync(groupOrder);

        var addressees = groupOrder.OrderItems.Select(o => o.UserId).Where(id => id != userId);

        var feedTasks = new List<Task>();

        foreach (var addressee in addressees)
        {
            if (groupOrder.Status == GroupOrderStatus.Delivered)
                feedTasks.Add(_graphService.SendActivityFeedTypeDelivered(userId, addressee, groupOrder.Id));
            else if (groupOrder.Status == GroupOrderStatus.Closed)
                feedTasks.Add(_graphService.SendActivityFeedTypeClosed(userId, addressee, groupOrder.RestaurantName, groupOrder.Id));
        }   

        await Task.WhenAll(feedTasks);
    }
}
