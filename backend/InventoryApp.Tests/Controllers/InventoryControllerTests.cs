using InventoryApp.Api.Controllers;
using InventoryApp.Api.DTOs;
using InventoryApp.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InventoryApp.Tests.Controllers;

public class InventoryControllerTests
{
    private readonly Mock<IInventoryService> _mockInventoryService;
    private readonly Mock<ILogger<InventoryController>> _mockLogger;
    private readonly InventoryController _controller;

    public InventoryControllerTests()
    {
        _mockInventoryService = new Mock<IInventoryService>();
        _mockLogger = new Mock<ILogger<InventoryController>>();
        _controller = new InventoryController(_mockInventoryService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task AddStock_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var productId = 1;
        var stockDto = new UpdateStockDto(10);
        var updatedInventory = new InventoryDto(
            productId, "Test Product", "SKU001", 20);

        _mockInventoryService
            .Setup(service => service.AddStockAsync(productId, stockDto))
            .ReturnsAsync(updatedInventory);

        // Act
        var result = await _controller.AddStock(productId, stockDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInventory = Assert.IsType<InventoryDto>(okResult.Value);
        Assert.Equal(20, returnedInventory.CurrentStock);
    }

    [Fact]
    public async Task AddStock_WithInvalidQuantity_ReturnsBadRequest()
    {
        // Arrange
        var productId = 1;
        var stockDto = new UpdateStockDto(-5);

        _mockInventoryService
            .Setup(service => service.AddStockAsync(productId, stockDto))
            .ThrowsAsync(new ArgumentException("Quantity must be positive when adding stock"));

        // Act
        var result = await _controller.AddStock(productId, stockDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Quantity must be positive when adding stock", badRequestResult.Value);
    }

    [Fact]
    public async Task RemoveStock_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var productId = 1;
        var stockDto = new UpdateStockDto(5);
        var updatedInventory = new InventoryDto(
            productId, "Test Product", "SKU001", 5);

        _mockInventoryService
            .Setup(service => service.RemoveStockAsync(productId, stockDto))
            .ReturnsAsync(updatedInventory);

        // Act
        var result = await _controller.RemoveStock(productId, stockDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInventory = Assert.IsType<InventoryDto>(okResult.Value);
        Assert.Equal(5, returnedInventory.CurrentStock);
    }

    [Fact]
    public async Task RemoveStock_WithInsufficientStock_ReturnsBadRequest()
    {
        // Arrange
        var productId = 1;
        var stockDto = new UpdateStockDto(15);

        _mockInventoryService
            .Setup(service => service.RemoveStockAsync(productId, stockDto))
            .ThrowsAsync(new InvalidOperationException("Insufficient stock"));

        // Act
        var result = await _controller.RemoveStock(productId, stockDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Insufficient stock", badRequestResult.Value);
    }

    [Fact]
    public async Task GetProductStock_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var productId = 1;
        var inventory = new InventoryDto(
            productId, "Test Product", "SKU001", 10);

        _mockInventoryService
            .Setup(service => service.GetProductStockAsync(productId))
            .ReturnsAsync(inventory);

        // Act
        var result = await _controller.GetProductStock(productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInventory = Assert.IsType<InventoryDto>(okResult.Value);
        Assert.Equal(10, returnedInventory.CurrentStock);
    }

    [Fact]
    public async Task GetProductStock_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var productId = 999;
        _mockInventoryService
            .Setup(service => service.GetProductStockAsync(productId))
            .ReturnsAsync(() => null);

        // Act
        var result = await _controller.GetProductStock(productId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
} 