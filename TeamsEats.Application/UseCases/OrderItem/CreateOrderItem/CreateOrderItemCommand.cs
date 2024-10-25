using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;
public record CreateOrderItemCommand(CreateOrderItemDTO CreateOrderItemDTO) : IRequest;
