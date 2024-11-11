using MediatR;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.UseCases;

public class CommentItemCommandHandler : IRequestHandler<CommentItemCommand>
{
    readonly IOrderRepository _orderRepository;
    readonly IGraphService _graphService;
    public CommentItemCommandHandler(IOrderRepository orderRepository, IGraphService graphService)
    {
        _orderRepository = orderRepository;
        _graphService = graphService;
    }
    public async Task Handle(CommentItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _orderRepository.GetItemAsync(request.ItemId);
        var message = request.Message;

        var addresserId = request.UserId;
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
