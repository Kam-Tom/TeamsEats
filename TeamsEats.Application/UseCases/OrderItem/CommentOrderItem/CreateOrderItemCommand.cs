using MediatR;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.UseCases;
public record CommentOrderItemCommand(CommentOrderItemDTO CommentOrderItemDTO) : IRequest;
