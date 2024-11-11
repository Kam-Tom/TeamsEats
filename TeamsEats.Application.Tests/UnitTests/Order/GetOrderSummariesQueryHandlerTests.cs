using Moq;
using AutoMapper;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;
using TeamsEats.Application.DTOs;
using TeamsEats.Application.UseCases;

namespace TeamsEats.Application.Tests.UnitTests;

public class GetOrderSummariesQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IGraphService> _graphServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetOrderSummariesQueryHandler _handler;

    public GetOrderSummariesQueryHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _graphServiceMock = new Mock<IGraphService>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetOrderSummariesQueryHandler(
            _orderRepositoryMock.Object,
            _graphServiceMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_MultipleOrders_ReturnsCorrectDTOs()
    {
        // Arrange
        var userId = "user1";
        var query = new GetOrderSummariesQuery(userId);
        
        var orders = new List<Order>
        {
            new Order 
            { 
                AuthorId = userId,
                Items = new List<Item> 
                { 
                    new Item { AuthorId = userId, Price = 50 } 
                }
            },
            new Order 
            { 
                AuthorId = "user2",
                Items = new List<Item> 
                { 
                    new Item { AuthorId = userId, Price = 30 } 
                }
            }
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrdersAsync())
            .ReturnsAsync(orders);
        _graphServiceMock.Setup(s => s.GetPhoto(It.IsAny<string>()))
            .ReturnsAsync("photoUrl");
        _mapperMock.Setup(m => m.Map<OrderSummaryDTO>(It.IsAny<Order>()))
            .Returns((Order o) => new OrderSummaryDTO());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.True(resultList[0].IsOwner);
        Assert.False(resultList[1].IsOwner);
        Assert.Equal(50, resultList[0].MyCost);
        Assert.Equal(30, resultList[1].MyCost);
    }

    [Fact]
    public async Task Handle_EmptyOrdersList_ReturnsEmptyCollection()
    {
        // Arrange
        var userId = "user1";
        var query = new GetOrderSummariesQuery(userId);
        
        _orderRepositoryMock.Setup(repo => repo.GetOrdersAsync())
            .ReturnsAsync(new List<Order>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_OrdersWithPhotos_SetsPhotosCorrectly()
    {
        // Arrange
        var userId = "user1";
        var query = new GetOrderSummariesQuery(userId);
        
        var orders = new List<Order>
        {
            new Order { AuthorId = "author1", Items = new List<Item>() },
            new Order { AuthorId = "author2", Items = new List<Item>() }
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrdersAsync())
            .ReturnsAsync(orders);
        _graphServiceMock.Setup(s => s.GetPhoto("author1"))
            .ReturnsAsync("photo1");
        _graphServiceMock.Setup(s => s.GetPhoto("author2"))
            .ReturnsAsync("photo2");
        _mapperMock.Setup(m => m.Map<OrderSummaryDTO>(It.IsAny<Order>()))
            .Returns(() => new OrderSummaryDTO());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var resultList = result.ToList();
        Assert.Equal("photo1", resultList[0].AuthorPhoto);
        Assert.Equal("photo2", resultList[1].AuthorPhoto);
        _graphServiceMock.Verify(s => s.GetPhoto(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_OrdersMapping_VerifyMapperCalls()
    {
        // Arrange
        var userId = "user1";
        var query = new GetOrderSummariesQuery(userId);
        
        var orders = new List<Order>
        {
            new Order() { Items = new List<Item>()},
            new Order() { Items = new List<Item>()}
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrdersAsync())
            .ReturnsAsync(orders);
        _graphServiceMock.Setup(s => s.GetPhoto(It.IsAny<string>()))
            .ReturnsAsync("photoUrl");
        _mapperMock.Setup(m => m.Map<OrderSummaryDTO>(It.IsAny<Order>()))
            .Returns(() => new OrderSummaryDTO());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mapperMock.Verify(m => m.Map<OrderSummaryDTO>(It.IsAny<Order>()), Times.Exactly(2));
    }
}