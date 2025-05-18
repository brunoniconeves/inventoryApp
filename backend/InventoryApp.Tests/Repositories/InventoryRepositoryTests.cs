using Microsoft.EntityFrameworkCore;
using InventoryApp.Api.Data;
using InventoryApp.Api.Models;
using InventoryApp.Api.Repositories;
using Xunit;

namespace InventoryApp.Tests.Repositories;

public class InventoryRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly InventoryRepository _repository;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public InventoryRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"InventoryDb_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(_options);
        _repository = new InventoryRepository(_context);
        
        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Test Product 1",
                Description = "Description 1",
                Price = 9.99m,
                SKU = "SKU001",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 2,
                Name = "Test Product 2",
                Description = "Description 2",
                Price = 19.99m,
                SKU = "SKU002",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var inventories = new List<Inventory>
        {
            new Inventory
            {
                Id = 1,
                ProductId = 1,
                CurrentStock = 10,
                LastUpdated = DateTime.UtcNow
            },
            new Inventory
            {
                Id = 2,
                ProductId = 2,
                CurrentStock = 20,
                LastUpdated = DateTime.UtcNow
            }
        };

        _context.Products.AddRange(products);
        _context.Inventory.AddRange(inventories);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByProductIdAsync_WithValidId_ReturnsInventory()
    {
        // Arrange
        var productId = 1;

        // Act
        var result = await _repository.GetByProductIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.ProductId);
        Assert.Equal(10, result.CurrentStock);
    }

    [Fact]
    public async Task GetByProductIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var invalidProductId = 999;

        // Act
        var result = await _repository.GetByProductIdAsync(invalidProductId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WithValidInventory_UpdatesSuccessfully()
    {
        // Arrange
        var inventory = await _context.Inventory.FirstAsync(i => i.ProductId == 1);
        inventory.CurrentStock = 15;

        // Act
        var result = await _repository.UpdateAsync(inventory);

        // Assert
        Assert.Equal(15, result.CurrentStock);
        
        var updatedInventory = await _context.Inventory.FindAsync(inventory.Id);
        Assert.NotNull(updatedInventory);
        Assert.Equal(15, updatedInventory!.CurrentStock);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllInventories()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        var inventoryList = result.ToList();
        Assert.NotNull(inventoryList);
        Assert.Equal(2, inventoryList.Count);
        Assert.Contains(inventoryList, i => i.ProductId == 1 && i.CurrentStock == 10);
        Assert.Contains(inventoryList, i => i.ProductId == 2 && i.CurrentStock == 20);
    }

    [Fact]
    public async Task AddAsync_WithValidInventory_AddsSuccessfully()
    {
        // Arrange
        var newInventory = new Inventory
        {
            Id = 3,
            ProductId = 1,
            CurrentStock = 30,
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(newInventory);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(30, result.CurrentStock);
        
        var addedInventory = await _context.Inventory.FindAsync(result.Id);
        Assert.NotNull(addedInventory);
        Assert.Equal(30, addedInventory!.CurrentStock);
    }

    [Fact]
    public async Task UpdateAsync_WithModifiedInventory_UpdatesLastUpdated()
    {
        // Arrange
        var inventory = await _context.Inventory.FirstAsync(i => i.ProductId == 1);
        var originalLastUpdated = inventory.LastUpdated;
        inventory.CurrentStock = 25;
        inventory.LastUpdated = DateTime.UtcNow.AddMinutes(5);

        // Act
        var result = await _repository.UpdateAsync(inventory);

        // Assert
        Assert.NotEqual(originalLastUpdated, result.LastUpdated);
        
        var updatedInventory = await _context.Inventory.FindAsync(inventory.Id);
        Assert.NotNull(updatedInventory);
        Assert.NotEqual(originalLastUpdated, updatedInventory!.LastUpdated);
    }

    [Fact]
    public async Task GetByProductIdAsync_IncludesProductData()
    {
        // Arrange
        var productId = 1;

        // Act
        var result = await _repository.GetByProductIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Product);
        Assert.Equal("Test Product 1", result.Product!.Name);
        Assert.Equal("SKU001", result.Product.SKU);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
} 