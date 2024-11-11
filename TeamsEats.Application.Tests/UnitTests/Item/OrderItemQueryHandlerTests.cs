using Moq;
using AutoMapper;
using TeamsEats.Domain.Interfaces;
using TeamsEats.Domain.Services;
using TeamsEats.Domain.Models;
using TeamsEats.Application.DTOs;
using TeamsEats.Application.UseCases;

namespace TeamsEats.Application.Tests.UnitTests;

public class ItemQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IGraphService> _graphServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ItemQueryHandler _handler;

    public ItemQueryHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _graphServiceMock = new Mock<IGraphService>();
        _mapperMock = new Mock<IMapper>();
        _handler = new ItemQueryHandler(
            _orderRepositoryMock.Object,
            _graphServiceMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsItemDTO()
    {
        // Arrange
        var itemId = 1;
        var userId = "user1";
        var query = new ItemQuery(itemId);
        
        var item = new Item { Id = itemId };
        var expectedItemDto = new ItemDTO { Id = itemId };

        _graphServiceMock.Setup(service => service.GetUserId())
            .ReturnsAsync(userId);
        _orderRepositoryMock.Setup(repo => repo.GetItemAsync(itemId))
            .ReturnsAsync(item);
        _mapperMock.Setup(mapper => mapper.Map<ItemDTO>(item))
            .Returns(expectedItemDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(expectedItemDto, result);
        _orderRepositoryMock.Verify(repo => repo.GetItemAsync(itemId), Times.Once);
        _graphServiceMock.Verify(service => service.GetUserId(), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<ItemDTO>(item), Times.Once);
    }

}