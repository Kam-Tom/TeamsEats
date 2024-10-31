using MediatR;
using TeamsEats.Application.DTOs;
namespace TeamsEats.Application.UseCases;

public record DeleteItemCommand(int ItemId, string UserId) : IRequest;
