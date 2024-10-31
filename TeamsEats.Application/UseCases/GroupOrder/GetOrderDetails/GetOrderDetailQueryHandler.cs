using AutoMapper;
using MediatR;
using TeamsEats.Application.DTOs;
using TeamsEats.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class GetGroupOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailQuery, OrderDetailsDTO>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IGraphService _graphService;
    private readonly IMapper _mapper;

    public GetGroupOrderDetailsQueryHandler(IOrderRepository orderRepository, IGraphService graphService, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
        _mapper = mapper;
    }

    public async Task<OrderDetailsDTO> Handle(GetOrderDetailQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderAsync(request.OrderId);
        var userId = request.UserId;

        var orderDetailsDTO = _mapper.Map<OrderDetailsDTO>(order);
        orderDetailsDTO.IsOwner = order.AuthorId == userId;
        orderDetailsDTO.AuthorPhoto = await _graphService.GetPhoto(order.AuthorId);
        orderDetailsDTO.MyCost = order.Items.Where(i => i.AuthorId == userId).Sum(i => i.Price);


        var itemDTOs = await Task.WhenAll(order.Items.Select(async item =>
        {
            var itemDTO = _mapper.Map<ItemDTO>(item);
            itemDTO.IsOwner = item.AuthorId == userId;
            itemDTO.AuthorPhoto = await _graphService.GetPhoto(item.AuthorId);
            return itemDTO;
        }));

        orderDetailsDTO.Items = itemDTOs.ToList();
        return orderDetailsDTO;
    }
}