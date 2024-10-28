using MediatR;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class DeleteItemCommandHandler: IRequestHandler<DeleteItemCommand>
{
    readonly IItemRepository _itemRepository;
    readonly IGraphService _graphService;
    public DeleteItemCommandHandler(IItemRepository itemRepository, IGraphService graphService)
    {
        _itemRepository = itemRepository;
        _graphService = graphService;
    }

    public async Task Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var userId = _graphService.GetUserId().Result;
        var item = _itemRepository.GetItemAsync(request.Id).Result;

        if (item.AuthorId != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to delete this item");
        }
        if (item.Order.Status != Domain.Enums.Status.Open)
        {
            throw new InvalidOperationException("You are not allowed to delete this item");
        }

        await _itemRepository.DeleteItemAsync(item);

    }

}
