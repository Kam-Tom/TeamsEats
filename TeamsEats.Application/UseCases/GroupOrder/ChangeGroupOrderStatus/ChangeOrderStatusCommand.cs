using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;

public record ChangeOrderStatusCommand(ChangeOrderStatusDTO ChangeOrderStatusDTO) : IRequest;

