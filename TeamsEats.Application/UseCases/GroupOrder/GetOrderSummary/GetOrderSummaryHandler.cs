using AutoMapper;
using MediatR;
using TeamsEats.Application.DTOs;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class GetOrderSummaryQueryHandler : IRequestHandler<GetOrderSummaryQuery, OrderSummaryDTO>
{
    readonly IOrderRepository _orderRepository;
    readonly IGraphService _graphService;
    private readonly IMapper _mapper;
    public GetOrderSummaryQueryHandler(IOrderRepository orderRepository, IGraphService graphService, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
        _mapper = mapper;
    }

    public async Task<OrderSummaryDTO> Handle(GetOrderSummaryQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderAsync(request.OrderId);
        var userId = request.UserId;

        var orderDTO = _mapper.Map<OrderSummaryDTO>(order);
        orderDTO.IsOwner = order.AuthorId == userId;
        orderDTO.MyCost = order.Items.Where(i => i.AuthorId == userId).Sum(i => i.Price);


        orderDTO.AuthorPhoto = await _graphService.GetPhoto(order.AuthorId);

        return orderDTO;
    }
}
