using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;

public record GetGroupOrdersLiteQuery() : IRequest<IEnumerable<GroupOrderLiteDTO>>;

