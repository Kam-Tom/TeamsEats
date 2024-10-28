using MediatR;
using TeamsEats.Application.DTOs;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class ItemQueryHandler: IRequestHandler<ItemQuery, ItemDTO>
{
    readonly IItemRepository _itemRepository;
    readonly IGraphService _graphService;
    public ItemQueryHandler(IItemRepository itemRepository, IGraphService graphService)
    {
        _itemRepository = itemRepository;
        _graphService = graphService;
    }

    public async Task<ItemDTO> Handle(ItemQuery request, CancellationToken cancellationToken)
    {
        var userId = await _graphService.GetUserId();
        var item = await _itemRepository.GetItemAsync(request.Id);

        if (item.AuthorId != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to delete this order item");
        }
        if (item.Order.Status != Domain.Enums.Status.Open)
        {
            throw new InvalidOperationException("You are not allowed to delete this order item");
        }

        return new ItemDTO()
        {
            Id = item.Id,
            AuthorName = item.AuthorName,
            Dish = item.Dish,
            Price = item.Price,
            IsOwner = item.AuthorId == userId,
            OrderId = item.OrderId,
            AdditionalInfo = item.AdditionalInfo
        };

    }

}
