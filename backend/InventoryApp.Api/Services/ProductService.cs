using InventoryApp.Api.DTOs;
using InventoryApp.Api.Models;
using InventoryApp.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IInventoryRepository _inventoryRepository;

    public ProductService(IProductRepository repository, IInventoryRepository inventoryRepository)
    {
        _repository = repository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _repository.GetAllAsync();
        var inventories = await _inventoryRepository.GetAllAsync();
        
        var inventoryMap = inventories.ToDictionary(i => i.ProductId, i => i.CurrentStock);
        
        return products.Select(p => new ProductDto(
            p.Id, p.Name, p.Description, p.Price,
            inventoryMap.GetValueOrDefault(p.Id, 0), p.SKU, p.CreatedAt, p.UpdatedAt));
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return null;

        var inventory = await _inventoryRepository.GetByProductIdAsync(id);
        return new ProductDto(
            product.Id, product.Name, product.Description, product.Price,
            inventory?.CurrentStock ?? 0, product.SKU, product.CreatedAt, product.UpdatedAt);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto productDto)
    {
        ValidateProduct(productDto);

        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            SKU = productDto.SKU,
            Price = productDto.Price,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdProduct = await _repository.AddAsync(product);

        // Create initial inventory
        var inventory = new Inventory
        {
            ProductId = createdProduct.Id,
            CurrentStock = productDto.StockQuantity,
            LastUpdated = DateTime.UtcNow
        };

        await _inventoryRepository.AddAsync(inventory);

        return new ProductDto(
            createdProduct.Id, createdProduct.Name, createdProduct.Description,
            createdProduct.Price, inventory.CurrentStock, createdProduct.SKU,
            createdProduct.CreatedAt, createdProduct.UpdatedAt);
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

        var updatedProduct = await _repository.UpdateAsync(existingProduct);
        var inventory = await _inventoryRepository.GetByProductIdAsync(id);

        return new ProductDto(
            updatedProduct.Id, updatedProduct.Name, updatedProduct.Description,
            updatedProduct.Price, inventory?.CurrentStock ?? 0, updatedProduct.SKU,
            updatedProduct.CreatedAt, updatedProduct.UpdatedAt);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        return await _repository.DeleteAsync(id);
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