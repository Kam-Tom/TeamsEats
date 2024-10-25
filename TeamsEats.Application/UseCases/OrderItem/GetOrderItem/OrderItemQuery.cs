using MediatR;
using TeamsEats.Application.DTOs;
namespace TeamsEats.Application.UseCases;

public record OrderItemQuery(int id) : IRequest<OrderItemDTO>;
