using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;

public record GetOrderSummaryQuery(int Id) : IRequest<OrderSummaryDTO>;

