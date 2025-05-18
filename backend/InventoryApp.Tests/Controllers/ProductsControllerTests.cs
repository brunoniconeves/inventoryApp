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
    public void Constructor_WithNullProductService_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new ProductsController(productService: null!, logger: Mock.Of<ILogger<ProductsController>>()));
        Assert.Equal("productService", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new ProductsController(Mock.Of<IProductService>(), logger: null!));
        Assert.Equal("logger", exception.ParamName);
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
    public async Task GetProducts_WhenServiceThrowsException_LogsErrorAndReturnsInternalServerError()
    {
        // Arrange
        _mockProductService
            .Setup(service => service.GetAllProductsAsync())
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving products", statusCodeResult.Value);
        VerifyLogError("Error retrieving products");
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
    public async Task GetProduct_WithZeroId_ReturnsBadRequest()
    {
        // Arrange
        var invalidId = 0;

        // Act
        var result = await _controller.GetProduct(invalidId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetProduct_WhenServiceThrowsException_LogsErrorAndReturnsInternalServerError()
    {
        // Arrange
        var productId = 1;
        _mockProductService
            .Setup(service => service.GetProductByIdAsync(productId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetProduct(productId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while retrieving the product", statusCodeResult.Value);
        VerifyLogError($"Error retrieving product {productId}");
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
        Assert.Equal(createDto.StockQuantity, returnedProduct.StockQuantity);
    }

    [Fact]
    public async Task CreateProduct_WithNullDto_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.CreateProduct(null!);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Product data is required", badRequestResult.Value);
    }

    [Theory]
    [InlineData("", "Description", 9.99, 10, "SKU001", "Product name is required")]
    [InlineData(" ", "Description", 9.99, 10, "SKU001", "Product name is required")]
    public async Task CreateProduct_WithInvalidName_ReturnsBadRequest(string name, string description, decimal price, int stockQuantity, string sku, string expectedError)
    {
        // Arrange
        var invalidProduct = new CreateProductDto(name, description, price, stockQuantity, sku);

        // Act
        var result = await _controller.CreateProduct(invalidProduct);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(expectedError, badRequestResult.Value);
    }

    [Theory]
    [InlineData("Test Product", "Description", 9.99, 10, "", "Product SKU is required")]
    [InlineData("Test Product", "Description", 9.99, 10, " ", "Product SKU is required")]
    public async Task CreateProduct_WithInvalidSKU_ReturnsBadRequestWithMessage(string name, string description, decimal price, int stockQuantity, string sku, string expectedError)
    {
        // Arrange
        var invalidProduct = new CreateProductDto(name, description, price, stockQuantity, sku);

        // Act
        var result = await _controller.CreateProduct(invalidProduct);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(expectedError, badRequestResult.Value);
    }

    [Theory]
    [InlineData("Test Product", "Description", 0, 10, "SKU001", "Product price must be greater than zero")]
    [InlineData("Test Product", "Description", -1, 10, "SKU001", "Product price must be greater than zero")]
    [InlineData("Test Product", "Description", -99.99, 10, "SKU001", "Product price must be greater than zero")]
    public async Task CreateProduct_WithInvalidPrice_ReturnsBadRequestWithMessage(string name, string description, decimal price, int stockQuantity, string sku, string expectedError)
    {
        // Arrange
        var invalidProduct = new CreateProductDto(name, description, price, stockQuantity, sku);

        // Act
        var result = await _controller.CreateProduct(invalidProduct);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(expectedError, badRequestResult.Value);
    }

    [Theory]
    [InlineData("Test Product", "Description", 9.99, -1, "SKU001", "Product stock quantity cannot be negative")]
    [InlineData("Test Product", "Description", 9.99, -100, "SKU001", "Product stock quantity cannot be negative")]
    public async Task CreateProduct_WithInvalidStockQuantity_ReturnsBadRequestWithMessage(string name, string description, decimal price, int stockQuantity, string sku, string expectedError)
    {
        // Arrange
        var invalidProduct = new CreateProductDto(name, description, price, stockQuantity, sku);

        // Act
        var result = await _controller.CreateProduct(invalidProduct);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(expectedError, badRequestResult.Value);
    }

    [Fact]
    public async Task CreateProduct_WithDuplicateSKU_ReturnsBadRequest()
    {
        // Arrange
        var newProduct = new CreateProductDto(
            "Test Product",
            "Description",
            9.99m,
            10,
            "SKU001");

        _mockProductService
            .Setup(service => service.CreateProductAsync(newProduct))
            .ThrowsAsync(new ArgumentException("Product with SKU 'SKU001' already exists"));

        // Act
        var result = await _controller.CreateProduct(newProduct);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Product with SKU 'SKU001' already exists", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateProduct_WithMaximumPrice_ReturnsBadRequest()
    {
        // Arrange
        var invalidProduct = new CreateProductDto(
            "Test Product",
            "Description",
            1000000.00m,  // Assuming this is above maximum allowed price
            10,
            "SKU001");

        _mockProductService
            .Setup(service => service.CreateProductAsync(invalidProduct))
            .ThrowsAsync(new ArgumentException("Product price exceeds maximum allowed value"));

        // Act
        var result = await _controller.CreateProduct(invalidProduct);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Product price exceeds maximum allowed value", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateProduct_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var productDto = new CreateProductDto("Test Product", "Description", 9.99m, 10, "SKU001");
        _mockProductService
            .Setup(service => service.CreateProductAsync(It.IsAny<CreateProductDto>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateProduct(productDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while creating the product", statusCodeResult.Value);
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

    [Theory]
    [InlineData("", "Description", 9.99, "SKU001", "Product name is required")]
    [InlineData(" ", "Description", 9.99, "SKU001", "Product name is required")]
    public async Task UpdateProduct_WithInvalidName_ReturnsBadRequest(string name, string description, decimal price, string sku, string expectedError)
    {
        // Arrange
        var invalidProduct = new UpdateProductDto(name, description, price, sku);

        // Act
        var result = await _controller.UpdateProduct(1, invalidProduct);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(expectedError, badRequestResult.Value);
    }

    [Theory]
    [InlineData("Test Product", "Description", 9.99, "", "Product SKU is required")]
    [InlineData("Test Product", "Description", 9.99, " ", "Product SKU is required")]
    public async Task UpdateProduct_WithInvalidSKU_ReturnsBadRequestWithMessage(string name, string description, decimal price, string sku, string expectedError)
    {
        // Arrange
        var invalidProduct = new UpdateProductDto(name, description, price, sku);

        // Act
        var result = await _controller.UpdateProduct(1, invalidProduct);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(expectedError, badRequestResult.Value);
    }

    [Theory]
    [InlineData("Test Product", "Description", 0, "SKU001", "Product price must be greater than zero")]
    [InlineData("Test Product", "Description", -1, "SKU001", "Product price must be greater than zero")]
    [InlineData("Test Product", "Description", -99.99, "SKU001", "Product price must be greater than zero")]
    public async Task UpdateProduct_WithInvalidPrice_ReturnsBadRequestWithMessage(string name, string description, decimal price, string sku, string expectedError)
    {
        // Arrange
        var productId = 1;
        var invalidProduct = new UpdateProductDto(name, description, price, sku);

        // Act
        var result = await _controller.UpdateProduct(productId, invalidProduct);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(expectedError, badRequestResult.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task UpdateProduct_WithInvalidId_ReturnsBadRequest(int invalidId)
    {
        // Arrange
        var updateDto = new UpdateProductDto(
            "Test Product",
            "Description",
            9.99m,
            "SKU001");

        // Act
        var result = await _controller.UpdateProduct(invalidId, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Product ID must be greater than zero", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateProduct_WithNullDto_ReturnsBadRequest()
    {
        // Arrange
        var productId = 1;

        // Act
        var result = await _controller.UpdateProduct(productId, null!);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Product data is required", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateProduct_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateProductDto(
            "Updated Product",
            "New Description",
            39.99m,
            "SKU002");

        _mockProductService
            .Setup(service => service.UpdateProductAsync(2, updateDto))
            .ThrowsAsync(new ArgumentException("Product ID mismatch"));

        // Act
        var result = await _controller.UpdateProduct(2, updateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Product ID mismatch", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateProduct_WithNonExistentProduct_ReturnsNotFound()
    {
        // Arrange
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type
        _mockProductService
            .Setup(service => service.UpdateProductAsync(999, It.IsAny<UpdateProductDto>()))
            .ReturnsAsync((ProductDto)null);
#pragma warning restore CS8600

        // Act
        var result = await _controller.UpdateProduct(999, new UpdateProductDto(
            "Updated Product",
            "New Description",
            39.99m,
            "SKU002"));

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateProduct_WhenConcurrentModification_ReturnsConflict()
    {
        // Arrange
        var productDto = new UpdateProductDto("Updated Product", "New Description", 19.99m, "SKU002");
        _mockProductService
            .Setup(service => service.UpdateProductAsync(1, It.IsAny<UpdateProductDto>()))
            .ThrowsAsync(new InvalidOperationException("Product has been modified by another user"));

        // Act
        var result = await _controller.UpdateProduct(1, productDto);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal("Product has been modified by another user", conflictResult.Value);
    }

    [Fact]
    public async Task UpdateProduct_WhenConcurrentModification_WithDifferentMessage_ReturnsInternalServerError()
    {
        // Arrange
        var productDto = new UpdateProductDto("Updated Product", "New Description", 19.99m, "SKU002");
        _mockProductService
            .Setup(service => service.UpdateProductAsync(1, It.IsAny<UpdateProductDto>()))
            .ThrowsAsync(new InvalidOperationException("Some other error"));

        // Act
        var result = await _controller.UpdateProduct(1, productDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while updating the product", statusCodeResult.Value);
        VerifyLogError("Error updating product 1");
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
    public async Task DeleteProduct_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        var invalidId = -1;

        // Act
        var result = await _controller.DeleteProduct(invalidId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Product ID must be greater than zero", badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteProduct_WithNonExistentId_ReturnsNotFound()
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
    public async Task DeleteProduct_WhenServiceThrowsException_LogsErrorAndReturnsInternalServerError()
    {
        // Arrange
        var productId = 1;
        _mockProductService
            .Setup(service => service.DeleteProductAsync(productId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteProduct(productId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while deleting the product", statusCodeResult.Value);
        VerifyLogError($"Error deleting product {productId}");
    }

    [Fact]
    public async Task DeleteProduct_WhenConcurrentModification_ReturnsConflict()
    {
        // Arrange
        _mockProductService
            .Setup(service => service.DeleteProductAsync(1))
            .ThrowsAsync(new InvalidOperationException("Product has been modified by another user"));

        // Act
        var result = await _controller.DeleteProduct(1);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal("Product has been modified by another user", conflictResult.Value);
    }

    private void VerifyLogError(string expectedMessage)
    {
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
} 