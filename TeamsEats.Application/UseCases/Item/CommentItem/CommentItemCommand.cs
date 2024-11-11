using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;
public record CommentItemCommand(string Message, int ItemId, string UserId) : IRequest;
