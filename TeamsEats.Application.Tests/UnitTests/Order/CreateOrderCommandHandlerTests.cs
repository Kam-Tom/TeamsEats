using Moq;
using AutoMapper;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Models;
using TeamsEats.Domain.Services;
using TeamsEats.Domain.Enums;
using TeamsEats.Application.DTOs;
using TeamsEats.Application.UseCases;

namespace TeamsEats.Application.Tests.UnitTests;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IGraphService> _graphServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _graphServiceMock = new Mock<IGraphService>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateOrderCommandHandler(
            _orderRepositoryMock.Object, 
            _graphServiceMock.Object, 
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesOrder()
    {
        // Arrange
        var userId = "user1";
        var userDisplayName = "User One";
        var orderId = 1;
        
        var createOrderDto = new CreateOrderDTO 
        { 
            DeliveryFee = 10,
            Restaurant = "Test Restaurant"
        };
        
        var command = new CreateOrderCommand(createOrderDto, userId);
        
        var expectedOrder = new Order
        {
            DeliveryFee = 10,
            Restaurant = "Test Restaurant"
        };

        _graphServiceMock.Setup(service => service.GetUserDisplayName(userId))
            .ReturnsAsync(userDisplayName);
        
        _mapperMock.Setup(mapper => mapper.Map<Order>(createOrderDto))
            .Returns(expectedOrder);
        
        _orderRepositoryMock.Setup(repo => repo.CreateOrderAsync(It.IsAny<Order>()))
            .ReturnsAsync(orderId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(orderId, result);
        
        _orderRepositoryMock.Verify(repo => repo.CreateOrderAsync(
            It.Is<Order>(o => 
                o.AuthorId == userId &&
                o.AuthorName == userDisplayName &&
                o.Status == Status.Open &&
                o.CurrentDeliveryFee == createOrderDto.DeliveryFee
            )), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_MapsOrderCorrectly()
    {
        // Arrange
        var userId = "user1";
        var createOrderDto = new CreateOrderDTO();
        var command = new CreateOrderCommand(createOrderDto, userId);
        var mappedOrder = new Order();

        _mapperMock.Setup(mapper => mapper.Map<Order>(createOrderDto))
            .Returns(mappedOrder);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapperMock.Verify();
    }
}