using System.Net.Http.Json;
using InventoryApp.Api.DTOs;
using InventoryApp.Api.Models;
using InventoryApp.Api.Repositories;
using InventoryApp.Api.Services;
using InventoryApp.Tests.Fixtures;
using Moq;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace InventoryApp.Tests.Services;

public class ProductServiceTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IInventoryRepository> _mockInventoryRepository;
    private readonly ProductService _service;

    public ProductServiceTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _mockProductRepository = new Mock<IProductRepository>();
        _mockInventoryRepository = new Mock<IInventoryRepository>();
        _service = new ProductService(_mockProductRepository.Object, _mockInventoryRepository.Object);
    }

    [Fact]
    public async Task GetAllProducts_Returns_Success_And_Seeded_Data()
    {
        // Arrange
        _factory.ResetDatabase();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/products");
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(products);
        Assert.Equal(2, products.Count);
        Assert.Contains(products, p => p.Name == "Test Product 1" && p.StockQuantity == 10);
        Assert.Contains(products, p => p.Name == "Test Product 2" && p.StockQuantity == 20);
    }

    [Fact]
    public async Task GetProductById_Returns_Correct_Product()
    {
        // Arrange
        _factory.ResetDatabase();
        var client = _factory.CreateClient();

        // First get all products to get a valid ID
        var allProducts = await client.GetFromJsonAsync<List<ProductDto>>("/api/products");
        Assert.NotNull(allProducts);

        var targetProduct = allProducts.FirstOrDefault(p => p.Name == "Test Product 1");
        Assert.NotNull(targetProduct);

        // Act
        var response = await client.GetAsync($"/api/products/{targetProduct.Id}");
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(product);
        Assert.Equal("Test Product 1", product.Name);
        Assert.Equal(9.99m, product.Price);
        Assert.Equal(10, product.StockQuantity);
    }

    [Fact]
    public async Task GetProductById_Returns_NotFound_For_NonExistent_Product()
    {
        // Arrange
        _factory.ResetDatabase();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/products/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_Returns_Created_And_Correct_Data()
    {
        // Arrange
        _factory.ResetDatabase();
        var client = _factory.CreateClient();
        var newProduct = new CreateProductDto(
            Name: "New Test Product",
            Description: "New Test Description",
            Price: 29.99m,
            StockQuantity: 30,
            SKU: "SKU003"
        );

        // Act
        var response = await client.PostAsJsonAsync("/api/products", newProduct);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(createdProduct);
        Assert.Equal(newProduct.Name, createdProduct.Name);
        Assert.Equal(newProduct.Price, createdProduct.Price);
        Assert.Equal(newProduct.StockQuantity, createdProduct.StockQuantity);
    }

    [Fact]
    public async Task UpdateProduct_Returns_Success_And_Updated_Data()
    {
        // Arrange
        _factory.ResetDatabase();
        var client = _factory.CreateClient();
        var allProducts = await client.GetFromJsonAsync<List<ProductDto>>("/api/products");
        Assert.NotNull(allProducts);
        Assert.NotEmpty(allProducts);

        var productToUpdate = allProducts[0];
        
        var updateDto = new UpdateProductDto(
            Name: "Updated Name",
            Description: productToUpdate.Description,
            Price: 39.99m,
            SKU: productToUpdate.SKU
        );

        // Act
        var response = await client.PutAsJsonAsync($"/api/products/{productToUpdate.Id}", updateDto);
        var updatedProduct = await response.Content.ReadFromJsonAsync<ProductDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updatedProduct);
        Assert.Equal("Updated Name", updatedProduct.Name);
        Assert.Equal(39.99m, updatedProduct.Price);
    }

    [Fact]
    public async Task DeleteProduct_Returns_NoContent_And_Removes_Product()
    {
        // Arrange
        _factory.ResetDatabase();
        var client = _factory.CreateClient();
        var allProducts = await client.GetFromJsonAsync<List<ProductDto>>("/api/products");
        Assert.NotNull(allProducts);
        Assert.NotEmpty(allProducts);

        var productToDelete = allProducts[0];

        // Act
        var response = await client.DeleteAsync($"/api/products/{productToDelete.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify product is deleted
        var getResponse = await client.GetAsync($"/api/products/{productToDelete.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { 
                Id = 1, 
                Name = "Test Product 1", 
                Description = "Description 1",
                Price = 9.99m,
                SKU = "SKU001",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new() { 
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
            new() {
                Id = 1,
                ProductId = 1,
                CurrentStock = 10,
                LastUpdated = DateTime.UtcNow
            },
            new() {
                Id = 2,
                ProductId = 2,
                CurrentStock = 20,
                LastUpdated = DateTime.UtcNow
            }
        };

        _mockProductRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(products);
        _mockInventoryRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(inventories);

        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Collection(result,
            dto => {
                Assert.Equal("Test Product 1", dto.Name);
                Assert.Equal(10, dto.StockQuantity);
            },
            dto => {
                Assert.Equal("Test Product 2", dto.Name);
                Assert.Equal(20, dto.StockQuantity);
            }
        );
    }

    [Fact]
    public async Task GetProductByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
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
            ProductId = 1,
            CurrentStock = 10,
            LastUpdated = DateTime.UtcNow
        };

        _mockProductRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(product);
        _mockInventoryRepository.Setup(repo => repo.GetByProductIdAsync(1))
            .ReturnsAsync(inventory);

        // Act
        var result = await _service.GetProductByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Price, result.Price);
        Assert.Equal(inventory.CurrentStock, result.StockQuantity);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockProductRepository.Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync(() => null);

        // Act
        var result = await _service.GetProductByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProductAsync_WithValidData_ReturnsCreatedProduct()
    {
        // Arrange
        var createDto = new CreateProductDto(
            "New Product", "Description", 29.99m, 30, "SKU003");

        var product = new Product
        {
            Id = 1,
            Name = createDto.Name,
            Description = createDto.Description ?? "",
            Price = createDto.Price,
            SKU = createDto.SKU ?? "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var inventory = new Inventory
        {
            Id = 1,
            ProductId = 1,
            CurrentStock = createDto.StockQuantity,
            LastUpdated = DateTime.UtcNow
        };

        _mockProductRepository.Setup(repo => repo.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(product);
        _mockInventoryRepository.Setup(repo => repo.AddAsync(It.IsAny<Inventory>()))
            .ReturnsAsync(inventory);

        // Act
        var result = await _service.CreateProductAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.Price, result.Price);
        Assert.Equal(createDto.StockQuantity, result.StockQuantity);
    }

    [Fact]
    public async Task UpdateProductAsync_WithValidData_ReturnsUpdatedProduct()
    {
        // Arrange
        var productId = 1;
        var updateDto = new UpdateProductDto(
            "Updated Product", "New Description", 39.99m, "SKU001");

        var existingProduct = new Product
        {
            Id = productId,
            Name = "Old Name",
            Description = "Old Description",
            Price = 29.99m,
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

        var updatedProduct = new Product
        {
            Id = productId,
            Name = updateDto.Name,
            Description = updateDto.Description ?? "",
            Price = updateDto.Price,
            SKU = updateDto.SKU ?? "",
            CreatedAt = existingProduct.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);
        _mockProductRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(updatedProduct);
        _mockInventoryRepository.Setup(repo => repo.GetByProductIdAsync(productId))
            .ReturnsAsync(existingInventory);

        // Act
        var result = await _service.UpdateProductAsync(productId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.Name, result.Name);
        Assert.Equal(updateDto.Price, result.Price);
        Assert.Equal(existingInventory.CurrentStock, result.StockQuantity);
    }

    [Fact]
    public async Task DeleteProductAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        var productId = 1;
        var existingProduct = new Product 
        { 
            Id = productId,
            Name = "Test Product",
            Description = "Test Description",
            Price = 9.99m,
            SKU = "SKU001",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);
        _mockProductRepository.Setup(repo => repo.DeleteAsync(productId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteProductAsync(productId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CreateProductAsync_WithInvalidName_ThrowsArgumentException()
    {
        // Arrange
        var invalidProduct = new CreateProductDto(
            "", "Description", 9.99m, 10, "SKU001");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateProductAsync(invalidProduct));
        Assert.Equal("Product name is required", exception.Message);
    }

    [Fact]
    public async Task CreateProductAsync_WithInvalidPrice_ThrowsArgumentException()
    {
        // Arrange
        var invalidProduct = new CreateProductDto(
            "Test Product", "Description", -1m, 10, "SKU001");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateProductAsync(invalidProduct));
        Assert.Equal("Product price must be greater than zero", exception.Message);
    }

    [Fact]
    public async Task CreateProductAsync_WithNegativeStock_ThrowsArgumentException()
    {
        // Arrange
        var invalidProduct = new CreateProductDto(
            "Test Product", "Description", 9.99m, -1, "SKU001");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateProductAsync(invalidProduct));
        Assert.Equal("Product stock quantity cannot be negative", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CreateProductAsync_WithEmptyDescription_ThrowsArgumentException(string? invalidDescription)
    {
        // Arrange
        var productDto = new CreateProductDto(
            "Test Product",
            invalidDescription,
            9.99m,
            10,
            "SKU001");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateProductAsync(productDto));
        Assert.Equal("Product description is required", exception.Message);
    }

    [Fact]
    public async Task CreateProductAsync_WithInvalidSKU_ThrowsArgumentException()
    {
        // Arrange
        var invalidProduct = new CreateProductDto(
            "Test Product", "Description", 9.99m, 10, "");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateProductAsync(invalidProduct));
        Assert.Equal("Product SKU is required", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task UpdateProductAsync_WithEmptyDescription_ThrowsArgumentException(string? invalidDescription)
    {
        // Arrange
        var existingProduct = new Product
        {
            Id = 1,
            Name = "Test Product",
            Description = "Original Description",
            Price = 9.99m,
            SKU = "SKU001",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(existingProduct);

        var updateDto = new UpdateProductDto(
            "Test Product",
            invalidDescription,
            9.99m,
            "SKU001");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.UpdateProductAsync(1, updateDto));
        Assert.Equal("Product description is required", exception.Message);
    }

    [Fact]
    public async Task UpdateProductAsync_WithInvalidSKU_ThrowsArgumentException()
    {
        // Arrange
        var productId = 1;
        var existingProduct = new Product
        {
            Id = productId,
            Name = "Old Name",
            Description = "Old Description",
            Price = 29.99m,
            SKU = "SKU001",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var updateDto = new UpdateProductDto(
            "Test Product", "Description", 9.99m, "");

        _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.UpdateProductAsync(productId, updateDto));
        Assert.Equal("Product SKU is required", exception.Message);
    }
} 