using AutoMapper;
using Moq;
using TeamsEats.Application.UseCases;
using TeamsEats.Application.DTOs;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;
using TeamsEats.Domain.Enums;

namespace TeamsEats.Application.Tests.UnitTests;

public class CreateItemCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IGraphService> _graphServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateItemCommandHandler _handler;

    public CreateItemCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _graphServiceMock = new Mock<IGraphService>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateItemCommandHandler(
            _orderRepositoryMock.Object,
            _graphServiceMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_AddsItemAndUpdatesOrder()
    {
        // Arrange
        var userId = "user1";
        var orderId = 1;
        var createItemDto = new CreateItemDTO { OrderId = orderId, Price = 10 };
        var command = new CreateItemCommand(createItemDto, userId);
        
        var existingOrder = new Order 
        { 
            Status = Status.Open,
            Items = new List<Item>(),
            MinimalPriceForFreeDelivery = 50,
            DeliveryFee = 10,
            CurrentPrice = 0
        };

        var newItem = new Item 
        { 
            Price = 10,
            OrderId = orderId,
            AuthorId = userId
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(orderId))
            .ReturnsAsync(existingOrder);
        _graphServiceMock.Setup(service => service.GetUserDisplayName(userId))
            .ReturnsAsync("User Name");
        _mapperMock.Setup(mapper => mapper.Map<Item>(createItemDto))
            .Returns(newItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _orderRepositoryMock.Verify(repo => repo.UpdateOrderAsync(It.Is<Order>(o => 
            o.CurrentPrice == 10 &&
            o.CurrentDeliveryFee == 10 &&
            o.Items.Count == 1)), Times.Once);
    }

    [Fact]
    public async Task Handle_OrderedOrder_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = "user1";
        var orderId = 1;
        var createItemDto = new CreateItemDTO { OrderId = orderId };
        var command = new CreateItemCommand(createItemDto, userId);
        
        var existingOrder = new Order 
        { 
            Status = Status.Ordered,
            Items = new List<Item>()
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(orderId))
            .ReturnsAsync(existingOrder);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _orderRepositoryMock.Verify(repo => repo.UpdateOrderAsync(It.IsAny<Order>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_TotalPriceExceedsMinimal_SetsDeliveryFeeToZero()
    {
        // Arrange
        var userId = "user1";
        var userDisplayName = "User One";
        var orderId = 1;
        
        var existingOrder = new Order 
        { 
            Status = Status.Open,
            Items = new List<Item> 
            {
                new Item { Price = 80, AuthorId = "user2" }
            },
            MinimalPriceForFreeDelivery = 100,
            DeliveryFee = 10,
            CurrentPrice = 80,
            CurrentDeliveryFee = 10
        };

        var createItemDto = new CreateItemDTO 
        { 
            OrderId = orderId,
            Price = 30
        };
        
        var command = new CreateItemCommand(createItemDto, userId);
        var mappedItem = new Item { Price = 30 };

        _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(orderId))
            .ReturnsAsync(existingOrder);
        _graphServiceMock.Setup(s => s.GetUserDisplayName(userId))
            .ReturnsAsync(userDisplayName);
        _mapperMock.Setup(m => m.Map<Item>(createItemDto))
            .Returns(mappedItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _orderRepositoryMock.Verify(repo => repo.UpdateOrderAsync(
            It.Is<Order>(o => 
                o.CurrentPrice == 110 && 
                o.CurrentDeliveryFee == 0 &&
                o.Items.Count == 2
            )), Times.Once);
    }
}