using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;

public record GetOrderSummaryQuery(int OrderId,string UserId) : IRequest<OrderSummaryDTO>;

