using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;
public record CreateItemCommand(CreateItemDTO CreateItemDTO, string UserId) : IRequest;
