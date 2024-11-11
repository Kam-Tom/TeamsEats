using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;

public record UpdateItemCommand(UpdateItemDTO UpdateItemDTO,int ItemId, string UserId) : IRequest;
