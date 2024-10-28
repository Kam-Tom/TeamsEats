using MediatR;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class CommentItemCommandHandler : IRequestHandler<CommentItemCommand>
{
    readonly IItemRepository _itemRepository;
    readonly IGraphService _graphService;
    public CommentItemCommandHandler(IItemRepository itemRepository, IGraphService graphService)
    {
        _itemRepository = itemRepository;
        _graphService = graphService;
    }
    public async Task Handle(CommentItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetItemAsync(request.CommentItemDTO.ItemId);
        var message = request.CommentItemDTO.Message;
        
        var addresserId = await _graphService.GetUserId();
        var addresseeId = item.AuthorId;

        if(item.Order.AuthorId != addresserId)
        {
            throw new UnauthorizedAccessException("You are not allowed to comment this order item");
        }
        if(item.AuthorId == addresserId)
        {
            throw new UnauthorizedAccessException("You are not allowed to comment your own order item");
        }

        await _graphService.SendMessage(addresserId, addresseeId, message);

    }
}
