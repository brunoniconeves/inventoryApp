using InventoryApp.Api.DTOs;
using InventoryApp.Api.Models;
using InventoryApp.Api.Repositories;

namespace InventoryApp.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(p => new ProductDto(
            p.Id, p.Name, p.Description, p.Price,
            p.StockQuantity, p.SKU, p.CreatedAt, p.UpdatedAt));
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return null;

        return new ProductDto(
            product.Id, product.Name, product.Description, product.Price,
            product.StockQuantity, product.SKU, product.CreatedAt, product.UpdatedAt);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto productDto)
    {
        ValidateProduct(productDto);

        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            StockQuantity = productDto.StockQuantity,
            SKU = productDto.SKU,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(product);
        return new ProductDto(
            created.Id, created.Name, created.Description, created.Price,
            created.StockQuantity, created.SKU, created.CreatedAt, created.UpdatedAt);
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto productDto)
    {
        var existingProduct = await _repository.GetByIdAsync(id);
        if (existingProduct == null) return null;

        ValidateProduct(productDto);

        existingProduct.Name = productDto.Name;
        existingProduct.Description = productDto.Description;
        existingProduct.Price = productDto.Price;
        existingProduct.SKU = productDto.SKU;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(existingProduct);
        if (updated == null) return null;

        return new ProductDto(
            updated.Id, updated.Name, updated.Description, updated.Price,
            updated.StockQuantity, updated.SKU, updated.CreatedAt, updated.UpdatedAt);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<ProductDto?> AddStockAsync(int id, UpdateStockDto stockDto)
    {
        if (stockDto.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be positive when adding stock");
        }

        var updated = await _repository.UpdateStockAsync(id, stockDto.Quantity);
        if (updated == null) return null;

        return new ProductDto(
            updated.Id, updated.Name, updated.Description, updated.Price,
            updated.StockQuantity, updated.SKU, updated.CreatedAt, updated.UpdatedAt);
    }

    public async Task<ProductDto?> RemoveStockAsync(int id, UpdateStockDto stockDto)
    {
        if (stockDto.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be positive when removing stock");
        }

        var updated = await _repository.UpdateStockAsync(id, -stockDto.Quantity);
        if (updated == null) return null;

        return new ProductDto(
            updated.Id, updated.Name, updated.Description, updated.Price,
            updated.StockQuantity, updated.SKU, updated.CreatedAt, updated.UpdatedAt);
    }

    private void ValidateProduct(CreateProductDto product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Product name is required");

        if (string.IsNullOrWhiteSpace(product.Description))
            throw new ArgumentException("Product description is required");

        if (string.IsNullOrWhiteSpace(product.SKU))
            throw new ArgumentException("Product SKU is required");

        if (product.Price <= 0)
            throw new ArgumentException("Product price must be greater than zero");

        if (product.StockQuantity < 0)
            throw new ArgumentException("Product stock quantity cannot be negative");
    }

    private void ValidateProduct(UpdateProductDto product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Product name is required");

        if (string.IsNullOrWhiteSpace(product.Description))
            throw new ArgumentException("Product description is required");

        if (string.IsNullOrWhiteSpace(product.SKU))
            throw new ArgumentException("Product SKU is required");

        if (product.Price <= 0)
            throw new ArgumentException("Product price must be greater than zero");
    }
} 