using Moq;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;
using TeamsEats.Domain.Enums;
using TeamsEats.Application.UseCases;
using TeamsEats.Application.DTOs;

namespace TeamsEats.Application.Tests.UnitTests;

public class UpdateItemCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IGraphService> _graphServiceMock;
    private readonly UpdateItemCommandHandler _handler;

    public UpdateItemCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _graphServiceMock = new Mock<IGraphService>();
        _handler = new UpdateItemCommandHandler(_orderRepositoryMock.Object, _graphServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesItemAndOrder()
    {
        // Arrange
        var userId = "user1";
        var itemId = 1;
        var updateItemDto = new UpdateItemDTO 
        { 
            Price = 60,
            Dish = "Updated Dish",
            AdditionalInfo = "Updated Info"
        };
        var command = new UpdateItemCommand(updateItemDto, itemId, userId);

        var existingItem = new Item 
        { 
            AuthorId = userId,
            Price = 50
        };
        
        var order = new Order 
        { 
            Status = Status.Open,
            Items = new List<Item> { existingItem },
            CurrentPrice = 50,
            MinimalPriceForFreeDelivery = 100,
            DeliveryFee = 10
        };
        
        existingItem.Order = order;

        _orderRepositoryMock.Setup(repo => repo.GetItemAsync(itemId))
            .ReturnsAsync(existingItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _orderRepositoryMock.Verify(repo => 
            repo.UpdateOrderAsync(It.Is<Order>(o => 
                o.CurrentPrice == 60 &&
                o.Items.First().Price == 60 &&
                o.Items.First().Dish == "Updated Dish" &&
                o.Items.First().AdditionalInfo == "Updated Info")),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UnauthorizedUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userId = "user1";
        var itemId = 1;
        var updateItemDto = new UpdateItemDTO();
        var command = new UpdateItemCommand(updateItemDto, itemId, userId);

        var existingItem = new Item 
        { 
            AuthorId = "differentUser",
            Order = new Order { Status = Status.Open }
        };

        _orderRepositoryMock.Setup(repo => repo.GetItemAsync(itemId))
            .ReturnsAsync(existingItem);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ClosedOrder_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = "user1";
        var itemId = 1;
        var updateItemDto = new UpdateItemDTO();
        var command = new UpdateItemCommand(updateItemDto, itemId, userId);

        var existingItem = new Item 
        { 
            AuthorId = userId,
            Order = new Order { Status = Status.Ordered }
        };

        _orderRepositoryMock.Setup(repo => repo.GetItemAsync(itemId))
            .ReturnsAsync(existingItem);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_PriceExceedsMinimal_SetsDeliveryFeeToZero()
    {
        // Arrange
        var userId = "user1";
        var itemId = 1;
        var updateItemDto = new UpdateItemDTO 
        { 
            Price = 120,
            Dish = "Updated Dish"
        };
        var command = new UpdateItemCommand(updateItemDto, itemId, userId);

        var existingItem = new Item 
        { 
            AuthorId = userId,
            Price = 50
        };
        
        var order = new Order 
        { 
            Status = Status.Open,
            Items = new List<Item> { existingItem },
            CurrentPrice = 50,
            MinimalPriceForFreeDelivery = 100,
            DeliveryFee = 10,
            CurrentDeliveryFee = 10
        };
        
        existingItem.Order = order;

        _orderRepositoryMock.Setup(repo => repo.GetItemAsync(itemId))
            .ReturnsAsync(existingItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _orderRepositoryMock.Verify(repo => 
            repo.UpdateOrderAsync(It.Is<Order>(o => 
                o.CurrentPrice == 120 && 
                o.CurrentDeliveryFee == 0)),
            Times.Once);
    }
}