using MediatR;
using TeamsEats.Application.DTOs;
namespace TeamsEats.Application.UseCases;
public record GetOrderDetailQuery(int Id) : IRequest<OrderDetailsDTO>;