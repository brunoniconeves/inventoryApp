using InventoryApp.Api;
using InventoryApp.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using InventoryApp.Api.Services;
using InventoryApp.Api.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Xunit;

namespace InventoryApp.Tests;

public class ProgramTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public ProgramTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public void CreateApp_RegistersRequiredServices()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        
        // Assert
        // Check if essential services are registered
        var productService = scope.ServiceProvider.GetService<IProductService>();
        var productRepository = scope.ServiceProvider.GetService<IProductRepository>();
        var inventoryRepository = scope.ServiceProvider.GetService<IInventoryRepository>();
        var dbContext = scope.ServiceProvider.GetService<Api.Data.ApplicationDbContext>();

        Assert.NotNull(productService);
        Assert.NotNull(productRepository);
        Assert.NotNull(inventoryRepository);
        Assert.NotNull(dbContext);
    }

    [Fact]
    public void CreateApp_ConfiguresCors()
    {
        // Arrange & Act
        var corsService = _factory.Services.GetService<ICorsService>();
        var corsPolicyProvider = _factory.Services.GetService<ICorsPolicyProvider>();

        // Assert
        Assert.NotNull(corsService);
        Assert.NotNull(corsPolicyProvider);
    }

    [Fact]
    public void CreateApp_UsesCorrectEnvironment()
    {
        // Arrange & Act
        var env = _factory.Services.GetService<IWebHostEnvironment>();

        // Assert
        Assert.NotNull(env);
        Assert.Equal("Testing", env.EnvironmentName);
    }

    [Fact]
    public void CreateApp_ConfiguresControllers()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var controllers = scope.ServiceProvider.GetService<IEnumerable<ControllerBase>>();

        // Assert
        Assert.NotNull(controllers);
    }
} 