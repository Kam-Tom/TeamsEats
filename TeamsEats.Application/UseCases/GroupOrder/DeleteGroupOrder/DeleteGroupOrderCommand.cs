using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;
public record DeleteGroupOrderCommand(int GroupOrderId) : IRequest;