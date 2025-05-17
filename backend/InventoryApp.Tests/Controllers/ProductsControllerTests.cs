using System.Net.Http.Json;
using InventoryApp.Api.Controllers;
using InventoryApp.Api.DTOs;
using InventoryApp.Api.Services;
using InventoryApp.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InventoryApp.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<ILogger<ProductsController>> _mockLogger;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _mockLogger = new Mock<ILogger<ProductsController>>();
        _controller = new ProductsController(_mockProductService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetProducts_ReturnsOkResult_WithProducts()
    {
        // Arrange
        var expectedProducts = new List<ProductDto>
        {
            new(1, "Test Product 1", "Description 1", 9.99m, 10, "SKU001", DateTime.UtcNow, DateTime.UtcNow),
            new(2, "Test Product 2", "Description 2", 19.99m, 20, "SKU002", DateTime.UtcNow, DateTime.UtcNow)
        };

        _mockProductService
            .Setup(service => service.GetAllProductsAsync())
            .ReturnsAsync(expectedProducts);

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Equal(expectedProducts.Count, returnedProducts.Count());
    }

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var productId = 1;
        var expectedProduct = new ProductDto(
            productId, "Test Product", "Description", 9.99m, 10, "SKU001", 
            DateTime.UtcNow, DateTime.UtcNow);

        _mockProductService
            .Setup(service => service.GetProductByIdAsync(productId))
            .ReturnsAsync(expectedProduct);

        // Act
        var result = await _controller.GetProduct(productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal(productId, returnedProduct.Id);
    }

    [Fact]
    public async Task GetProduct_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var productId = 999;
        _mockProductService
            .Setup(service => service.GetProductByIdAsync(productId))
            .ReturnsAsync(() => null);

        // Act
        var result = await _controller.GetProduct(productId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var createDto = new CreateProductDto(
            "New Product", "Description", 29.99m, 30, "SKU003");
        
        var createdProduct = new ProductDto(
            1, createDto.Name, createDto.Description, createDto.Price,
            createDto.StockQuantity, createDto.SKU, DateTime.UtcNow, DateTime.UtcNow);

        _mockProductService
            .Setup(service => service.CreateProductAsync(createDto))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.CreateProduct(createDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedProduct = Assert.IsType<ProductDto>(createdAtActionResult.Value);
        Assert.Equal(createDto.Name, returnedProduct.Name);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var productId = 1;
        var updateDto = new UpdateProductDto(
            "Updated Product", "New Description", 39.99m, "SKU001");
        
        var updatedProduct = new ProductDto(
            productId, updateDto.Name, updateDto.Description, updateDto.Price,
            10, updateDto.SKU, DateTime.UtcNow, DateTime.UtcNow);

        _mockProductService
            .Setup(service => service.UpdateProductAsync(productId, updateDto))
            .ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.UpdateProduct(productId, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal(updateDto.Name, returnedProduct.Name);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var productId = 1;
        _mockProductService
            .Setup(service => service.DeleteProductAsync(productId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteProduct(productId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var productId = 999;
        _mockProductService
            .Setup(service => service.DeleteProductAsync(productId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteProduct(productId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetProducts_WhenExceptionOccurs_ReturnsInternalServerError()
    {
        // Arrange
        _mockProductService
            .Setup(service => service.GetAllProductsAsync())
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving products", statusCodeResult.Value);
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
    public async Task CreateProduct_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidProduct = new CreateProductDto(
            "", "", -1m, -1, "");
        
        _mockProductService
            .Setup(service => service.CreateProductAsync(invalidProduct))
            .ThrowsAsync(new ArgumentException("Product validation failed"));

        // Act
        var result = await _controller.CreateProduct(invalidProduct);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Product validation failed", badRequestResult.Value);
    }

    [Fact]
    public async Task AddStock_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var productId = 1;
        var stockDto = new UpdateStockDto(10);
        var updatedProduct = new ProductDto(
            productId, "Test Product", "Description", 9.99m, 20, "SKU001", 
            DateTime.UtcNow, DateTime.UtcNow);

        _mockProductService
            .Setup(service => service.AddStockAsync(productId, stockDto))
            .ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.AddStock(productId, stockDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal(20, returnedProduct.StockQuantity);
    }

    [Fact]
    public async Task AddStock_WithInvalidQuantity_ReturnsBadRequest()
    {
        // Arrange
        var productId = 1;
        var stockDto = new UpdateStockDto(-5);

        _mockProductService
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
        var updatedProduct = new ProductDto(
            productId, "Test Product", "Description", 9.99m, 5, "SKU001", 
            DateTime.UtcNow, DateTime.UtcNow);

        _mockProductService
            .Setup(service => service.RemoveStockAsync(productId, stockDto))
            .ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.RemoveStock(productId, stockDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal(5, returnedProduct.StockQuantity);
    }

    [Fact]
    public async Task RemoveStock_WithInsufficientStock_ReturnsBadRequest()
    {
        // Arrange
        var productId = 1;
        var stockDto = new UpdateStockDto(15);

        _mockProductService
            .Setup(service => service.RemoveStockAsync(productId, stockDto))
            .ThrowsAsync(new InvalidOperationException("Insufficient stock"));

        // Act
        var result = await _controller.RemoveStock(productId, stockDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Insufficient stock", badRequestResult.Value);
    }
} 