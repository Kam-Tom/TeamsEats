using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;
public record DeleteOrderCommand(int OrderId, string UserId) : IRequest;