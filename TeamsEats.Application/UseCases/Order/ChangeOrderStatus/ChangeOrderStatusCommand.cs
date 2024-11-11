using MediatR;
using TeamsEats.Application.DTOs;
using TeamsEats.Domain.Enums;

namespace TeamsEats.Application.UseCases;

public record ChangeOrderStatusCommand(Status Status, int OrderId, string UserId) : IRequest;

