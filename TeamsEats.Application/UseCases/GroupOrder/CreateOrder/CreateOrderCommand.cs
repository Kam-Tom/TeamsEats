using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;

public record CreateOrderCommand(CreateOrderDTO CreateOrderDTO, string UserId) : IRequest<int>;

