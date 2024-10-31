using AutoMapper;
using MediatR;
using TeamsEats.Application.DTOs;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class ItemQueryHandler: IRequestHandler<ItemQuery, ItemDTO>
{
    readonly IOrderRepository _orderRepository;
    readonly IGraphService _graphService;
    readonly IMapper _mapper;
    public ItemQueryHandler(IOrderRepository orderRepository, IGraphService graphService, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
        _mapper = mapper;
    }

    public async Task<ItemDTO> Handle(ItemQuery request, CancellationToken cancellationToken)
    {
        var userId = await _graphService.GetUserId();
        var item = await _orderRepository.GetItemAsync(request.ItemId);

        return _mapper.Map<ItemDTO>(item);

    }

}
