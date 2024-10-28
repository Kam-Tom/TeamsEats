using MediatR;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand>
{
    readonly IItemRepository _itemRepository;
    readonly IGraphService _graphService;
    public UpdateItemCommandHandler(IItemRepository itemRepository, IGraphService graphService)
    {
        _itemRepository = itemRepository;
        _graphService = graphService;
    }
    public async Task Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {

        var userID = await _graphService.GetUserId();
        var item = await _itemRepository.GetItemAsync(request.UpdateItemDTO.Id);

        if (item.AuthorId != userID)
        {
            throw new UnauthorizedAccessException("You are not allowed to update this order item");
        }
        if(item.Order.Status != Domain.Enums.Status.Open)
        {
            throw new InvalidOperationException("You are not allowed to update this order item");
        }

        item.Price = request.UpdateItemDTO.Price;
        item.AdditionalInfo = request.UpdateItemDTO.AdditionalInfo;
        item.Dish = request.UpdateItemDTO.Dish;

        await _itemRepository.UpdateItemAsync(item);
    }
}
