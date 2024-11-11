using Moq;
using AutoMapper;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;
using TeamsEats.Application.DTOs;
using TeamsEats.Application.UseCases;

namespace TeamsEats.Application.Tests.UnitTests;

public class GetOrderDetailsQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IGraphService> _graphServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetGroupOrderDetailsQueryHandler _handler;

    public GetOrderDetailsQueryHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _graphServiceMock = new Mock<IGraphService>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetGroupOrderDetailsQueryHandler(
            _orderRepositoryMock.Object,
            _graphServiceMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_OrderOwner_ReturnsCorrectDTO()
    {
        // Arrange
        var orderId = 1;
        var userId = "user1";
        var query = new GetOrderDetailQuery(orderId, userId);
        
        var order = new Order 
        { 
            AuthorId = userId,
            Items = new List<Item> 
            {
                new Item { AuthorId = userId, Price = 50 },
                new Item { AuthorId = "user2", Price = 30 }
            }
        };

        var orderDto = new OrderDetailsDTO();
        var itemDto = new ItemDTO();

        _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(orderId))
            .ReturnsAsync(order);
        _mapperMock.Setup(m => m.Map<OrderDetailsDTO>(order))
            .Returns(orderDto);
        _mapperMock.Setup(m => m.Map<ItemDTO>(It.IsAny<Item>()))
            .Returns(itemDto);
        _graphServiceMock.Setup(s => s.GetPhoto(It.IsAny<string>()))
            .ReturnsAsync("photoUrl");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsOwner);
        Assert.Equal(50, result.MyCost);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task Handle_NonOwner_ReturnsCorrectDTO()
    {
        // Arrange
        var orderId = 1;
        var userId = "user2";
        var query = new GetOrderDetailQuery(orderId, userId);
        
        var order = new Order 
        { 
            AuthorId = "user1",
            Items = new List<Item> 
            {
                new Item { AuthorId = userId, Price = 30 }
            }
        };

        var orderDto = new OrderDetailsDTO();
        var itemDto = new ItemDTO();

        _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(orderId))
            .ReturnsAsync(order);
        _mapperMock.Setup(m => m.Map<OrderDetailsDTO>(order))
            .Returns(orderDto);
        _mapperMock.Setup(m => m.Map<ItemDTO>(It.IsAny<Item>()))
            .Returns(itemDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsOwner);
        Assert.Equal(30, result.MyCost);
    }

    [Fact]
    public async Task Handle_ItemsMapping_SetsCorrectOwnership()
    {
        // Arrange
        var orderId = 1;
        var userId = "user1";
        var query = new GetOrderDetailQuery(orderId, userId);
        
        var order = new Order 
        { 
            Items = new List<Item> 
            {
                new Item { AuthorId = userId },
                new Item { AuthorId = "user2" }
            }
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(orderId))
            .ReturnsAsync(order);
        _mapperMock.Setup(m => m.Map<OrderDetailsDTO>(order))
            .Returns(new OrderDetailsDTO());
        _mapperMock.Setup(m => m.Map<ItemDTO>(It.IsAny<Item>()))
            .Returns(() => new ItemDTO());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Items[0].IsOwner);
        Assert.False(result.Items[1].IsOwner);
    }

    [Fact]
    public async Task Handle_PhotoRetrieval_CallsGraphService()
    {
        // Arrange
        var orderId = 1;
        var userId = "user1";
        var query = new GetOrderDetailQuery(orderId, userId);
        
        var order = new Order 
        { 
            AuthorId = "author1",
            Items = new List<Item> 
            {
                new Item { AuthorId = "user2" }
            }
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(orderId))
            .ReturnsAsync(order);
        _mapperMock.Setup(m => m.Map<OrderDetailsDTO>(order))
            .Returns(new OrderDetailsDTO());
        _mapperMock.Setup(m => m.Map<ItemDTO>(It.IsAny<Item>()))
            .Returns(new ItemDTO());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _graphServiceMock.Verify(s => s.GetPhoto("author1"), Times.Once);
        _graphServiceMock.Verify(s => s.GetPhoto("user2"), Times.Once);
    }
}