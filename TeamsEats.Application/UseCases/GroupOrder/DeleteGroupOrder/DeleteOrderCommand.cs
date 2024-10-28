using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;
public record DeleteOrderCommand(int Id) : IRequest;