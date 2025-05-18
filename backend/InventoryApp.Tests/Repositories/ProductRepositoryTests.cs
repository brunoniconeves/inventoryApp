using InventoryApp.Api.Data;
using InventoryApp.Api.Models;
using InventoryApp.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InventoryApp.Tests.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _repository;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public ProductRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"ProductDb_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(_options);
        _repository = new ProductRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() {
                Name = "Test Product 1",
                Description = "Description 1",
                Price = 9.99m,
                SKU = "SKU001",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new() {
                Name = "Test Product 2",
                Description = "Description 2",
                Price = 19.99m,
                SKU = "SKU002",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.Name == "Test Product 1" && p.Price == 9.99m);
        Assert.Contains(result, p => p.Name == "Test Product 2" && p.Price == 19.99m);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Description",
            Price = 9.99m,
            SKU = "SKU001",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Price, result.Price);
        Assert.Equal(product.SKU, result.SKU);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WithValidProduct_AddsSuccessfully()
    {
        // Arrange
        var product = new Product
        {
            Name = "New Product",
            Description = "New Description",
            Price = 29.99m,
            SKU = "SKU003",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(product);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Price, result.Price);

        // Verify in database
        var dbProduct = await _context.Products.FindAsync(result.Id);
        Assert.NotNull(dbProduct);
        Assert.Equal(product.Name, dbProduct.Name);
    }

    [Fact]
    public async Task UpdateAsync_WithValidProduct_UpdatesSuccessfully()
    {
        // Arrange
        var product = new Product
        {
            Name = "Original Name",
            Description = "Original Description",
            Price = 9.99m,
            SKU = "SKU001",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        product.Name = "Updated Name";
        product.Price = 19.99m;

        // Act
        var result = await _repository.UpdateAsync(product);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal(19.99m, result.Price);

        // Verify in database
        var dbProduct = await _context.Products.FindAsync(product.Id);
        Assert.NotNull(dbProduct);
        Assert.Equal("Updated Name", dbProduct.Name);
        Assert.Equal(19.99m, dbProduct.Price);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentProduct_ReturnsNull()
    {
        // Arrange
        var product = new Product
        {
            Id = 999,
            Name = "Non-existent Product",
            Description = "Description",
            Price = 9.99m,
            SKU = "SKU001",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.UpdateAsync(product);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
    {
        // Arrange
        var product = new Product
        {
            Name = "Product to Delete",
            Description = "Description",
            Price = 9.99m,
            SKU = "SKU001",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(product.Id);

        // Assert
        Assert.True(result);
        var dbProduct = await _context.Products.FindAsync(product.Id);
        Assert.Null(dbProduct);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }
} 