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

    [Fact]
    public async Task GetProductStock_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        int productId = 1;
        _mockInventoryService
            .Setup(s => s.GetProductStockAsync(productId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetProductStock(productId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving the stock information", statusCodeResult.Value);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task AddStock_WhenServiceReturnsNull_ReturnsNotFound()
    {
        // Arrange
        int productId = 1;
        var stockDto = new UpdateStockDto(10);
        _mockInventoryService
            .Setup(s => s.AddStockAsync(productId, stockDto))
            .ReturnsAsync((InventoryDto?)null);

        // Act
        var result = await _controller.AddStock(productId, stockDto);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task AddStock_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        int productId = 1;
        var stockDto = new UpdateStockDto(10);
        _mockInventoryService
            .Setup(s => s.AddStockAsync(productId, stockDto))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.AddStock(productId, stockDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while adding stock", statusCodeResult.Value);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task RemoveStock_WhenServiceReturnsNull_ReturnsNotFound()
    {
        // Arrange
        int productId = 1;
        var stockDto = new UpdateStockDto(5);
        _mockInventoryService
            .Setup(s => s.RemoveStockAsync(productId, stockDto))
            .ReturnsAsync((InventoryDto?)null);

        // Act
        var result = await _controller.RemoveStock(productId, stockDto);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task RemoveStock_WhenServiceThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        int productId = 1;
        var stockDto = new UpdateStockDto(5);
        var errorMessage = "Invalid quantity";
        _mockInventoryService
            .Setup(s => s.RemoveStockAsync(productId, stockDto))
            .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _controller.RemoveStock(productId, stockDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(errorMessage, badRequestResult.Value);
    }

    [Fact]
    public async Task RemoveStock_WhenServiceThrowsInvalidOperationException_ReturnsBadRequest()
    {
        // Arrange
        int productId = 1;
        var stockDto = new UpdateStockDto(5);
        var errorMessage = "Insufficient stock";
        _mockInventoryService
            .Setup(s => s.RemoveStockAsync(productId, stockDto))
            .ThrowsAsync(new InvalidOperationException(errorMessage));

        // Act
        var result = await _controller.RemoveStock(productId, stockDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(errorMessage, badRequestResult.Value);
    }

    [Fact]
    public async Task RemoveStock_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        int productId = 1;
        var stockDto = new UpdateStockDto(5);
        _mockInventoryService
            .Setup(s => s.RemoveStockAsync(productId, stockDto))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.RemoveStock(productId, stockDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while removing stock", statusCodeResult.Value);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task GetProductStock_WhenServiceReturnsNull_ReturnsNotFound()
    {
        // Arrange
        int productId = 1;
        _mockInventoryService
            .Setup(s => s.GetProductStockAsync(productId))
            .ReturnsAsync((InventoryDto?)null);

        // Act
        var result = await _controller.GetProductStock(productId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetProductStock_WhenServiceReturnsInventory_ReturnsOk()
    {
        // Arrange
        int productId = 1;
        var inventory = new InventoryDto(
            productId,
            "Test Product",
            "TEST-SKU",
            10
        );
        _mockInventoryService
            .Setup(s => s.GetProductStockAsync(productId))
            .ReturnsAsync(inventory);

        // Act
        var result = await _controller.GetProductStock(productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInventory = Assert.IsType<InventoryDto>(okResult.Value);
        Assert.Equal(inventory.ProductId, returnedInventory.ProductId);
        Assert.Equal(inventory.CurrentStock, returnedInventory.CurrentStock);
    }

    [Fact]
    public async Task AddStock_WhenServiceReturnsInventory_ReturnsOk()
    {
        // Arrange
        int productId = 1;
        var stockDto = new UpdateStockDto(10);
        var inventory = new InventoryDto(
            productId,
            "Test Product",
            "TEST-SKU",
            20
        );
        _mockInventoryService
            .Setup(s => s.AddStockAsync(productId, stockDto))
            .ReturnsAsync(inventory);

        // Act
        var result = await _controller.AddStock(productId, stockDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInventory = Assert.IsType<InventoryDto>(okResult.Value);
        Assert.Equal(inventory.ProductId, returnedInventory.ProductId);
        Assert.Equal(inventory.CurrentStock, returnedInventory.CurrentStock);
    }

    [Fact]
    public async Task RemoveStock_WhenServiceReturnsInventory_ReturnsOk()
    {
        // Arrange
        int productId = 1;
        var stockDto = new UpdateStockDto(5);
        var inventory = new InventoryDto(
            productId,
            "Test Product",
            "TEST-SKU",
            5
        );
        _mockInventoryService
            .Setup(s => s.RemoveStockAsync(productId, stockDto))
            .ReturnsAsync(inventory);

        // Act
        var result = await _controller.RemoveStock(productId, stockDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedInventory = Assert.IsType<InventoryDto>(okResult.Value);
        Assert.Equal(inventory.ProductId, returnedInventory.ProductId);
        Assert.Equal(inventory.CurrentStock, returnedInventory.CurrentStock);
    }

    [Fact]
    public async Task AddStock_WhenServiceThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        int productId = 1;
        var stockDto = new UpdateStockDto(10);
        var errorMessage = "Invalid quantity";
        _mockInventoryService
            .Setup(s => s.AddStockAsync(productId, stockDto))
            .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _controller.AddStock(productId, stockDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(errorMessage, badRequestResult.Value);
    }
} 