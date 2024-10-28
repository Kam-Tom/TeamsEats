using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;

public record CreateOrderCommand(CreateOrderDTO CreateOrderDTO) : IRequest<int>;

