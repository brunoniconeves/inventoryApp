using System.ComponentModel.DataAnnotations;
using InventoryApp.Api.Models;
using Xunit;

namespace InventoryApp.Tests.Models;

public class InventoryTests
{
    [Fact]
    public void Inventory_Properties_SetAndGetCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var inventory = new Inventory
        {
            Id = 1,
            ProductId = 1,
            CurrentStock = 10,
            LastUpdated = now,
            Product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "Test Description",
                Price = 9.99m,
                SKU = "SKU001",
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        // Act & Assert
        Assert.Equal(1, inventory.Id);
        Assert.Equal(1, inventory.ProductId);
        Assert.Equal(10, inventory.CurrentStock);
        Assert.Equal(now, inventory.LastUpdated);
        Assert.NotNull(inventory.Product);
        Assert.Equal("Test Product", inventory.Product.Name);
    }

    [Fact]
    public void Inventory_RequiredAttributes_ArePresent()
    {
        // Arrange
        var propertyNames = new[] { "ProductId", "CurrentStock" };
        var inventoryType = typeof(Inventory);

        // Act & Assert
        foreach (var propertyName in propertyNames)
        {
            var property = inventoryType.GetProperty(propertyName);
            Assert.NotNull(property);
            var requiredAttr = property!.GetCustomAttributes(typeof(RequiredAttribute), false);
            Assert.Single(requiredAttr);
        }
    }

    [Fact]
    public void Inventory_ConcurrencyCheck_IsPresent()
    {
        // Arrange
        var inventoryType = typeof(Inventory);
        var lastUpdatedProperty = inventoryType.GetProperty("LastUpdated");

        // Act & Assert
        Assert.NotNull(lastUpdatedProperty);
        var concurrencyAttr = lastUpdatedProperty!.GetCustomAttributes(typeof(ConcurrencyCheckAttribute), false);
        Assert.Single(concurrencyAttr);
    }

    [Fact]
    public void Inventory_NavigationProperty_IsConfiguredCorrectly()
    {
        // Arrange
        var inventoryType = typeof(Inventory);
        var productProperty = inventoryType.GetProperty("Product");

        // Act & Assert
        Assert.NotNull(productProperty);
        Assert.Equal(typeof(Product), productProperty!.PropertyType);
    }

    [Fact]
    public void Inventory_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var inventory = new Inventory();

        // Assert
        Assert.Equal(0, inventory.Id);
        Assert.Equal(0, inventory.ProductId);
        Assert.Equal(0, inventory.CurrentStock);
        Assert.Equal(default(DateTime), inventory.LastUpdated);
        Assert.Null(inventory.Product);
    }

    [Fact]
    public void Inventory_WithNullProduct_AllowsNullNavigation()
    {
        // Arrange
        var inventory = new Inventory
        {
            Id = 1,
            ProductId = 1,
            CurrentStock = 10,
            LastUpdated = DateTime.UtcNow
        };

        // Act & Assert
        Assert.Null(inventory.Product);
    }
} 