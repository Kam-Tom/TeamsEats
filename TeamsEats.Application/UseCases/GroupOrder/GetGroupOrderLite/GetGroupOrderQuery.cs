using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;

public record GetGroupOrderLiteQuery(int GroupOrderId) : IRequest<GroupOrderLiteDTO>;

