using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;

public record ChangeGroupOrdersStatusCommand(ChangeGroupOrderStatusDTO dto) : IRequest;

