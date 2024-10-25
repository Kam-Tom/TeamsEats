using MediatR;
using TeamsEats.Application.DTOs;
namespace TeamsEats.Application.UseCases;

public record DeleteOrderItemCommand(int OrderItemId) : IRequest;
