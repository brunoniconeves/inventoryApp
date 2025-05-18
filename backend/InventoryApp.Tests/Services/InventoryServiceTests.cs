using InventoryApp.Api.DTOs;
using InventoryApp.Api.Models;
using InventoryApp.Api.Repositories;
using InventoryApp.Api.Services;
using Moq;
using Xunit;

namespace InventoryApp.Tests.Services;

public class InventoryServiceTests
{
    private readonly Mock<IInventoryRepository> _mockInventoryRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        _mockInventoryRepository = new Mock<IInventoryRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _service = new InventoryService(_mockInventoryRepository.Object, _mockProductRepository.Object);
    }

    [Fact]
    public async Task AddStockAsync_WithValidQuantity_UpdatesStock()
    {
        // Arrange
        var productId = 1;
        var stockDto = new UpdateStockDto(10);
        
        var existingProduct = new Product
        {
            Id = productId,
            Name = "Test Product",
            Description = "Description",
            Price = 9.99m,
            SKU = "SKU001",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var existingInventory = new Inventory
        {
            Id = 1,
            ProductId = productId,
            CurrentStock = 5,
            LastUpdated = DateTime.UtcNow
        };

        var updatedInventory = new Inventory
        {
            Id = 1,
            ProductId = productId,
            CurrentStock = existingInventory.CurrentStock + stockDto.Quantity,
            LastUpdated = DateTime.UtcNow
        };

        _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);
        _mockInventoryRepository.Setup(repo => repo.GetByProductIdAsync(productId))
            .ReturnsAsync(existingInventory);
        _mockInventoryRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Inventory>()))
            .ReturnsAsync(updatedInventory);

        // Act
        var result = await _service.AddStockAsync(productId, stockDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15, result.CurrentStock);
        Assert.Equal(existingProduct.Name, result.ProductName);
        Assert.Equal(existingProduct.SKU, result.SKU);
    }

    [Fact]
    public async Task AddStockAsync_WithNegativeQuantity_ThrowsArgumentException()
    {
        // Arrange
        var productId = 1;
        var stockDto = new UpdateStockDto(-5);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.AddStockAsync(productId, stockDto));
        Assert.Equal("Quantity must be positive when adding stock", exception.Message);
    }

    [Fact]
    public async Task RemoveStockAsync_WithValidQuantity_UpdatesStock()
    {
        // Arrange
        var productId = 1;
        var stockDto = new UpdateStockDto(5);
        
        var existingProduct = new Product
        {
            Id = productId,
            Name = "Test Product",
            Description = "Description",
            Price = 9.99m,
            SKU = "SKU001",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var existingInventory = new Inventory
        {
            Id = 1,
            ProductId = productId,
            CurrentStock = 10,
            LastUpdated = DateTime.UtcNow
        };

        var updatedInventory = new Inventory
        {
            Id = 1,
            ProductId = productId,
            CurrentStock = existingInventory.CurrentStock - stockDto.Quantity,
            LastUpdated = DateTime.UtcNow
        };

        _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);
        _mockInventoryRepository.Setup(repo => repo.GetByProductIdAsync(productId))
            .ReturnsAsync(existingInventory);
        _mockInventoryRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Inventory>()))
            .ReturnsAsync(updatedInventory);

        // Act
        var result = await _service.RemoveStockAsync(productId, stockDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.CurrentStock);
        Assert.Equal(existingProduct.Name, result.ProductName);
        Assert.Equal(existingProduct.SKU, result.SKU);
    }

    [Fact]
    public async Task RemoveStockAsync_WithNegativeQuantity_ThrowsArgumentException()
    {
        // Arrange
        var productId = 1;
        var stockDto = new UpdateStockDto(-5);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.RemoveStockAsync(productId, stockDto));
        Assert.Equal("Quantity must be positive when removing stock", exception.Message);
    }

    [Fact]
    public async Task RemoveStockAsync_WithInsufficientStock_ThrowsInvalidOperationException()
    {
        // Arrange
        var productId = 1;
        var stockDto = new UpdateStockDto(15);
        
        var existingProduct = new Product
        {
            Id = productId,
            Name = "Test Product",
            Description = "Description",
            Price = 9.99m,
            SKU = "SKU001",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var existingInventory = new Inventory
        {
            Id = 1,
            ProductId = productId,
            CurrentStock = 10,
            LastUpdated = DateTime.UtcNow
        };

        _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);
        _mockInventoryRepository.Setup(repo => repo.GetByProductIdAsync(productId))
            .ReturnsAsync(existingInventory);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.RemoveStockAsync(productId, stockDto));
        Assert.Equal("Insufficient stock available", exception.Message);
    }

    [Fact]
    public async Task GetProductStockAsync_WithValidId_ReturnsInventory()
    {
        // Arrange
        var productId = 1;
        var existingProduct = new Product
        {
            Id = productId,
            Name = "Test Product",
            Description = "Description",
            Price = 9.99m,
            SKU = "SKU001",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var inventory = new Inventory
        {
            Id = 1,
            ProductId = productId,
            CurrentStock = 10,
            LastUpdated = DateTime.UtcNow
        };

        _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);
        _mockInventoryRepository.Setup(repo => repo.GetByProductIdAsync(productId))
            .ReturnsAsync(inventory);

        // Act
        var result = await _service.GetProductStockAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.ProductId);
        Assert.Equal(10, result.CurrentStock);
        Assert.Equal(existingProduct.Name, result.ProductName);
        Assert.Equal(existingProduct.SKU, result.SKU);
    }

    [Fact]
    public async Task GetProductStockAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var productId = 999;
        _mockInventoryRepository.Setup(repo => repo.GetByProductIdAsync(productId))
            .ReturnsAsync(() => null);

        // Act
        var result = await _service.GetProductStockAsync(productId);

        // Assert
        Assert.Null(result);
    }
} 