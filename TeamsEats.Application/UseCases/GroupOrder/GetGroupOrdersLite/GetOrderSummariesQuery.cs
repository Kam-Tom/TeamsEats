using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;

public record GetOrderSummariesQuery() : IRequest<IEnumerable<OrderSummaryDTO>>;

