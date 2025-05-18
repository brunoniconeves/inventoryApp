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
        db.Products.RemoveRange(db.Products);
        db.SaveChanges();

        // Seed with test data
        db.Products.AddRange(
            new Product 
            { 
                Name = "Test Product 1", 
                Description = "Test Description 1",
                Price = 9.99m,
                StockQuantity = 10,
                SKU = "SKU001",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product 
            { 
                Name = "Test Product 2", 
                Description = "Test Description 2",
                Price = 19.99m,
                StockQuantity = 20,
                SKU = "SKU002",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
        db.SaveChanges();
    }
} 