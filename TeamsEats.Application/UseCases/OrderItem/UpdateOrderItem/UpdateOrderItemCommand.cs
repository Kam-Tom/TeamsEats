using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;

public record UpdateOrderItemCommand(UpdateOrderItemDTO UpdateOrderItemDTO) : IRequest;
