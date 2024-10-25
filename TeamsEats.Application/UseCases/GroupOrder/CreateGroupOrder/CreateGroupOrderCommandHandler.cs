using MediatR;
using TeamsEats.Application.UseCases;
using TeamsEats.Domain.Enums;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;

public class CreateGroupOrderCommandHandler : IRequestHandler<CreateGroupOrderCommand, int>
{
    private readonly IGroupOrderRepository _groupOrderRepository;
    private readonly IGraphService _graphService;

    public CreateGroupOrderCommandHandler(
        IGroupOrderRepository groupOrderRepository,
        IGraphService graphService)
    {
        _groupOrderRepository = groupOrderRepository;
        _graphService = graphService;
    }

    public async Task<int> Handle(CreateGroupOrderCommand request, CancellationToken cancellationToken)
    {
        var userID = await _graphService.GetUserID();
        var userDisplayName = await _graphService.GetUserDisplayName(userID);

        var groupOrder = new GroupOrder()
        {
            UserId = userID,
            UserDisplayName = userDisplayName,
            PhoneNumber = request.CreateGroupOrderDTO.PhoneNumber,
            RestaurantName = request.CreateGroupOrderDTO.RestaurantName,
            BankAccount = request.CreateGroupOrderDTO.BankAccount,
            MinimalPrice = request.CreateGroupOrderDTO.MinimalPrice,
            DeliveryFee = request.CreateGroupOrderDTO.DeliveryFee,
            MinimalPriceForFreeDelivery = request.CreateGroupOrderDTO.MinimalPriceForFreeDelivery,
            Status = GroupOrderStatus.Open,
            ClosingTime = request.CreateGroupOrderDTO.ClosingTime
        };

        await _groupOrderRepository.CreateGroupOrderAsync(groupOrder);

        return groupOrder.Id;
    }
}
