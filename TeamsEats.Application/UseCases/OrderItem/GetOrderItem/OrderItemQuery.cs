using MediatR;
using TeamsEats.Application.DTOs;
namespace TeamsEats.Application.UseCases;

public record ItemQuery(int Id) : IRequest<ItemDTO>;
