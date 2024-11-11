using MediatR;
using TeamsEats.Application.DTOs;
namespace TeamsEats.Application.UseCases;
public record GetOrderDetailQuery(int OrderId, string UserId) : IRequest<OrderDetailsDTO>;
