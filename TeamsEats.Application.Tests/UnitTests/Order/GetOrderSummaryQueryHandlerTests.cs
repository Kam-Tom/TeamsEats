using Moq;
using AutoMapper;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;
using TeamsEats.Application.DTOs;
using TeamsEats.Application.UseCases;

namespace TeamsEats.Application.Tests.UnitTests;

public class GetOrderSummaryQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IGraphService> _graphServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetOrderSummaryQueryHandler _handler;

    public GetOrderSummaryQueryHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _graphServiceMock = new Mock<IGraphService>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetOrderSummaryQueryHandler(
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
        var query = new GetOrderSummaryQuery(orderId, userId);
        
        var order = new Order 
        { 
            AuthorId = userId,
            Items = new List<Item> 
            { 
                new Item { AuthorId = userId, Price = 50 }
            }
        };

        var expectedDto = new OrderSummaryDTO();

        _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(orderId))
            .ReturnsAsync(order);
        _mapperMock.Setup(m => m.Map<OrderSummaryDTO>(order))
            .Returns(expectedDto);
        _graphServiceMock.Setup(s => s.GetPhoto(userId))
            .ReturnsAsync("photoUrl");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsOwner);
        Assert.Equal(50, result.MyCost);
        Assert.Equal("photoUrl", result.AuthorPhoto);
    }

    [Fact]
    public async Task Handle_NonOwner_ReturnsCorrectDTO()
    {
        // Arrange
        var orderId = 1;
        var userId = "user2";
        var query = new GetOrderSummaryQuery(orderId, userId);
        
        var order = new Order 
        { 
            AuthorId = "user1",
            Items = new List<Item> 
            { 
                new Item { AuthorId = userId, Price = 30 }
            }
        };

        var expectedDto = new OrderSummaryDTO();

        _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(orderId))
            .ReturnsAsync(order);
        _mapperMock.Setup(m => m.Map<OrderSummaryDTO>(order))
            .Returns(expectedDto);
        _graphServiceMock.Setup(s => s.GetPhoto("user1"))
            .ReturnsAsync("photoUrl");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsOwner);
        Assert.Equal(30, result.MyCost);
        Assert.Equal("photoUrl", result.AuthorPhoto);
    }

    [Fact]
    public async Task Handle_VerifyMappingAndPhotoRetrieval()
    {
        // Arrange
        var orderId = 1;
        var userId = "user1";
        var query = new GetOrderSummaryQuery(orderId, userId);
        
        var order = new Order 
        { 
            AuthorId = "author1",
            Items = new List<Item>()
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(orderId))
            .ReturnsAsync(order);
        _mapperMock.Setup(m => m.Map<OrderSummaryDTO>(order))
            .Returns(new OrderSummaryDTO());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mapperMock.Verify(m => m.Map<OrderSummaryDTO>(order), Times.Once);
        _graphServiceMock.Verify(s => s.GetPhoto("author1"), Times.Once);
    }
}