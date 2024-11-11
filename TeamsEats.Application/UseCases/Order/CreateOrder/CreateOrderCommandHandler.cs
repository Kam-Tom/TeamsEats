using AutoMapper;
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
    private readonly IMapper _mapper;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IGraphService graphService, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
        _mapper = mapper;

    }

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userID = request.UserId;
        var userDisplayName = await _graphService.GetUserDisplayName(userID);

        var order = _mapper.Map<Order>(request.CreateOrderDTO);
        order.AuthorId = userID;
        order.AuthorName = userDisplayName;
        order.Status = Status.Open;
        order.CurrentDeliveryFee = request.CreateOrderDTO.DeliveryFee;

        var id = await _orderRepository.CreateOrderAsync(order);

        return id;
    }
}
