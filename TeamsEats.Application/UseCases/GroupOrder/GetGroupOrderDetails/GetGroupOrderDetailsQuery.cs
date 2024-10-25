using MediatR;
using TeamsEats.Application.DTOs;
namespace TeamsEats.Application.UseCases;
public record GetGroupOrderDetailsQuery(int GroupOrderId) : IRequest<GroupOrderDetailsDTO>;
