using InventoryApp.Api;
using InventoryApp.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InventoryApp.Api.Models;

namespace InventoryApp.Tests.Fixtures;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"InMemoryTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove the app's ApplicationDbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add ApplicationDbContext using an in-memory database for testing
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });
        });
    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clear all data
        db.Inventory.RemoveRange(db.Inventory);
        db.Products.RemoveRange(db.Products);
        db.SaveChanges();

        // Create test products
        var products = new List<Product>
        {
            new Product 
            { 
                Id = 1,
                Name = "Test Product 1", 
                Description = "Test Description 1",
                Price = 9.99m,
                SKU = "SKU001",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product 
            { 
                Id = 2,
                Name = "Test Product 2", 
                Description = "Test Description 2",
                Price = 19.99m,
                SKU = "SKU002",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        // Add products
        db.Products.AddRange(products);
        db.SaveChanges();

        // Add inventory records
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

        db.Inventory.AddRange(inventories);
        db.SaveChanges();
    }
} 