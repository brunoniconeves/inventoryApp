using InventoryApp.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace InventoryApp.Tests;

public class ProgramTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Products_Endpoint_Returns_Success()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Products_Returns_Empty_List_Initially()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/products");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal("[]", content);
    }

    [Theory]
    [InlineData("/api/products")]
    [InlineData("/api/products/1")]
    public async Task Endpoints_Return_Success(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        Assert.True(response.StatusCode is HttpStatusCode.OK or HttpStatusCode.NotFound);
    }
} 