using MediatR;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand>
{
    readonly IItemRepository _itemRepository;
    readonly IGraphService _graphService;
    public CreateItemCommandHandler(IItemRepository itemRepository, IGraphService graphService)
    {
        _itemRepository = itemRepository;
        _graphService = graphService;
    }
    public async Task Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var userID = await _graphService.GetUserId();
        var userDisplayName = await _graphService.GetUserDisplayName(userID);

        var orderItem = new Item()
        {
            AuthorId = userID,
            AuthorName = userDisplayName,
            Dish = request.CreateItemDTO.Dish,
            Price = request.CreateItemDTO.Price,
            AdditionalInfo = request.CreateItemDTO.AdditionalInfo,
            OrderId = request.CreateItemDTO.OrderId
        };

        await _itemRepository.CreateItemAsync(orderItem);

    }
}
