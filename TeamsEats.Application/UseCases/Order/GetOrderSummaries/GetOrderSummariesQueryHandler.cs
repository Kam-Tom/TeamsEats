using AutoMapper;
using MediatR;
using TeamsEats.Application.DTOs;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class GetOrderSummariesQueryHandler : IRequestHandler<GetOrderSummariesQuery, IEnumerable<OrderSummaryDTO>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IGraphService _graphService;
    private readonly IMapper _mapper;

    public GetOrderSummariesQueryHandler(IOrderRepository orderRepository, IGraphService graphService, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderSummaryDTO>> Handle(GetOrderSummariesQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetOrdersAsync();
        var userId = request.UserId;

        var orderDTOs = orders.Select(order =>
        {
            var orderDTO = _mapper.Map<OrderSummaryDTO>(order);
            orderDTO.IsOwner = order.AuthorId == userId;
            orderDTO.MyCost = order.Items.Where(i=> i.AuthorId == userId).Sum(i => i.Price);
            return orderDTO;
        }).ToList();


        var photoTasks = orders.Select(order => _graphService.GetPhoto(order.AuthorId)).ToList();
        var photos = await Task.WhenAll(photoTasks);

        for (int i = 0; i < orderDTOs.Count; i++)
        {
            orderDTOs[i].AuthorPhoto = photos[i];
        }

        return orderDTOs;
    }
}
