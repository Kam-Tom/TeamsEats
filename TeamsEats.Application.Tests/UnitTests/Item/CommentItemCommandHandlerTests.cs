using Moq;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Application.UseCases;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;

namespace TeamsEats.Application.Tests.UnitTests;

public class CommentItemCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IGraphService> _graphServiceMock;
    private readonly CommentItemCommandHandler _handler;

    public CommentItemCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _graphServiceMock = new Mock<IGraphService>();
        _handler = new CommentItemCommandHandler(_orderRepositoryMock.Object, _graphServiceMock.Object);

    }

    [Fact]
    public async Task Handle_ValidRequest_SendsMessage()
    {
        // Arrange
        var command = new CommentItemCommand("Hello",1,"Adresser");
        var item = new Item { AuthorId = "Adressee", Order = new Order { AuthorId = "Adresser"} };

        _orderRepositoryMock.Setup(repo => repo.GetItemAsync(command.ItemId)).ReturnsAsync(item);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _graphServiceMock.Verify(service => service.SendMessage("Adresser", "Adressee", "Hello"), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotOrderAuthor_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var command = new CommentItemCommand("Hello", 1, "Adresser");
        var item = new Item 
        { 
            AuthorId = "Adressee", 
            Order = new Order { AuthorId = "DifferentUser"} 
        };

        _orderRepositoryMock.Setup(repo => repo.GetItemAsync(command.ItemId))
            .ReturnsAsync(item);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _handler.Handle(command, CancellationToken.None));

        _graphServiceMock.Verify(service => 
            service.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), 
            Times.Never);
    }

    [Fact]
    public async Task Handle_UserCommentingOwnItem_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var command = new CommentItemCommand("Hello", 1, "Adresser");
        var item = new Item 
        { 
            AuthorId = "Adresser", 
            Order = new Order { AuthorId = "Adresser"} 
        };

        _orderRepositoryMock.Setup(repo => repo.GetItemAsync(command.ItemId))
            .ReturnsAsync(item);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _handler.Handle(command, CancellationToken.None));

        _graphServiceMock.Verify(service => 
            service.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), 
            Times.Never);
    }
}