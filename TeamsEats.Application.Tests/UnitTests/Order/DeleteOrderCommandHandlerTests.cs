using Moq;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;
using TeamsEats.Domain.Enums;
using TeamsEats.Application.UseCases;

namespace TeamsEats.Application.Tests.UnitTests;

public class DeleteOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IGraphService> _graphServiceMock;
    private readonly DeleteOrderCommandHandler _handler;

    public DeleteOrderCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _graphServiceMock = new Mock<IGraphService>();
        _handler = new DeleteOrderCommandHandler(_orderRepositoryMock.Object, _graphServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DeletesOrder()
    {
        // Arrange
        var userId = "user1";
        var orderId = 1;
        var command = new DeleteOrderCommand(orderId, userId);
        
        var order = new Order 
        { 
            AuthorId = userId,
            Status = Status.Open,
            Items = new List<Item>()
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(orderId))
            .ReturnsAsync(order);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _orderRepositoryMock.Verify(repo => repo.DeleteOrderAsync(order), Times.Once);
    }

    [Fact]
    public async Task Handle_UnauthorizedUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userId = "unauthorizedUser";
        var orderId = 1;
        var command = new DeleteOrderCommand(orderId, userId);
        
        var order = new Order { AuthorId = "differentUser" };

        _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(orderId))
            .ReturnsAsync(order);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _handler.Handle(command, CancellationToken.None));
        
        _orderRepositoryMock.Verify(repo => repo.DeleteOrderAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NonDeliveredOrder_SendsNotifications()
    {
        // Arrange
        var userId = "user1";
        var orderId = 1;
        var command = new DeleteOrderCommand(orderId, userId);
        
        var order = new Order 
        { 
            AuthorId = userId,
            Status = Status.Open,
            Restaurant = "TestRestaurant",
            Items = new List<Item> 
            { 
                new Item { AuthorId = "user2" },
                new Item { AuthorId = "user3" },
                new Item { AuthorId = "user2" } // Duplicate user to test distinct
            }
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(orderId))
            .ReturnsAsync(order);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _graphServiceMock.Verify(service => 
            service.SendActivityFeedTypeDeleted(userId, "user2", "TestRestaurant"), 
            Times.Once);
        _graphServiceMock.Verify(service => 
            service.SendActivityFeedTypeDeleted(userId, "user3", "TestRestaurant"), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeliveredOrder_DoesNotSendNotifications()
    {
        // Arrange
        var userId = "user1";
        var orderId = 1;
        var command = new DeleteOrderCommand(orderId, userId);
        
        var order = new Order 
        { 
            AuthorId = userId,
            Status = Status.Delivered,
            Items = new List<Item> 
            { 
                new Item { AuthorId = "user2" }
            }
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(orderId))
            .ReturnsAsync(order);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _graphServiceMock.Verify(service => 
            service.SendActivityFeedTypeDeleted(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()), 
            Times.Never);
    }
}