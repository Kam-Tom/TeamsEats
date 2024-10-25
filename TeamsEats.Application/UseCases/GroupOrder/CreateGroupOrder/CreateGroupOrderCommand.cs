using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;

public record CreateGroupOrderCommand(CreateGroupOrderDTO CreateGroupOrderDTO) : IRequest<int>;

