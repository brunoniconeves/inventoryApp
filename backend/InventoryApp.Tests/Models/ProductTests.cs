using System.ComponentModel.DataAnnotations;
using InventoryApp.Api.Models;
using Xunit;

namespace InventoryApp.Tests.Models;

public class ProductTests
{
    [Fact]
    public void Product_Properties_SetAndGetCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            Price = 9.99m,
            SKU = "SKU001",
            CreatedAt = now,
            UpdatedAt = now
        };

        // Act & Assert
        Assert.Equal(1, product.Id);
        Assert.Equal("Test Product", product.Name);
        Assert.Equal("Test Description", product.Description);
        Assert.Equal(9.99m, product.Price);
        Assert.Equal("SKU001", product.SKU);
        Assert.Equal(now, product.CreatedAt);
        Assert.Equal(now, product.UpdatedAt);
        Assert.Null(product.Inventory);
    }

    [Fact]
    public void Product_RequiredAttributes_ArePresent()
    {
        // Arrange
        var propertyNames = new[] { "Name", "Description", "Price", "SKU" };
        var productType = typeof(Product);

        // Act & Assert
        foreach (var propertyName in propertyNames)
        {
            var property = productType.GetProperty(propertyName);
            Assert.NotNull(property);
            var requiredAttr = property!.GetCustomAttributes(typeof(RequiredAttribute), false);
            Assert.Single(requiredAttr);
        }
    }

    [Fact]
    public void Product_MaxLengthAttributes_AreCorrect()
    {
        // Arrange
        var maxLengths = new Dictionary<string, int>
        {
            { "Name", 200 },
            { "Description", 2000 },
            { "SKU", 50 }
        };
        var productType = typeof(Product);

        // Act & Assert
        foreach (var (propertyName, expectedLength) in maxLengths)
        {
            var property = productType.GetProperty(propertyName);
            Assert.NotNull(property);
            var maxLengthAttr = property!.GetCustomAttributes(typeof(MaxLengthAttribute), false).FirstOrDefault() as MaxLengthAttribute;
            Assert.NotNull(maxLengthAttr);
            Assert.Equal(expectedLength, maxLengthAttr!.Length);
        }
    }

    [Fact]
    public void Product_ConcurrencyCheck_IsPresent()
    {
        // Arrange
        var productType = typeof(Product);
        var updatedAtProperty = productType.GetProperty("UpdatedAt");

        // Act & Assert
        Assert.NotNull(updatedAtProperty);
        var concurrencyAttr = updatedAtProperty!.GetCustomAttributes(typeof(ConcurrencyCheckAttribute), false);
        Assert.Single(concurrencyAttr);
    }

    [Fact]
    public void Product_NavigationProperty_IsConfiguredCorrectly()
    {
        // Arrange
        var productType = typeof(Product);
        var inventoryProperty = productType.GetProperty("Inventory");

        // Act & Assert
        Assert.NotNull(inventoryProperty);
        Assert.Equal(typeof(Inventory), inventoryProperty!.PropertyType);
    }
} 