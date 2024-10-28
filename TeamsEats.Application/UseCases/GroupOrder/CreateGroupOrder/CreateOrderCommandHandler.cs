using MediatR;
using TeamsEats.Application.UseCases;
using TeamsEats.Domain.Enums;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IGraphService _graphService;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IGraphService graphService)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
    }

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userID = await _graphService.GetUserId();
        var userDisplayName = await _graphService.GetUserDisplayName(userID);

        var groupOrder = new Order()
        {
            AuthorId = userID,
            AuthorName = userDisplayName,
            PhoneNumber = request.CreateOrderDTO.PhoneNumber,
            Restaurant = request.CreateOrderDTO.Restaurant,
            BankAccount = request.CreateOrderDTO.BankAccount,
            MinimalPrice = request.CreateOrderDTO.MinimalPrice,
            DeliveryFee = request.CreateOrderDTO.DeliveryFee,
            MinimalPriceForFreeDelivery = request.CreateOrderDTO.MinimalPriceForFreeDelivery,
            Status = Status.Open,
            ClosingTime = request.CreateOrderDTO.ClosingTime
        };

        await _orderRepository.CreateOrderAsync(groupOrder);

        return groupOrder.Id;
    }
}
