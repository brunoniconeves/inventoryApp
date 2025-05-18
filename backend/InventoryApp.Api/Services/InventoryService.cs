using InventoryApp.Api.DTOs;
using InventoryApp.Api.Models;
using InventoryApp.Api.Repositories;

namespace InventoryApp.Api.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProductRepository _productRepository;

    public InventoryService(IInventoryRepository inventoryRepository, IProductRepository productRepository)
    {
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
    }

    public async Task<InventoryDto?> GetProductStockAsync(int productId)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
        if (inventory == null) return null;

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) return null;

        return new InventoryDto(
            productId,
            product.Name,
            product.SKU,
            inventory.CurrentStock
        );
    }

    public async Task<InventoryDto?> AddStockAsync(int productId, UpdateStockDto stockDto)
    {
        if (stockDto.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be positive when adding stock");
        }

        var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
        if (inventory == null) return null;

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) return null;

        // Add stock
        inventory.CurrentStock += stockDto.Quantity;
        inventory.LastUpdated = DateTime.UtcNow;

        // Update inventory
        await _inventoryRepository.UpdateAsync(inventory);

        return new InventoryDto(
            productId,
            product.Name,
            product.SKU,
            inventory.CurrentStock
        );
    }

    public async Task<InventoryDto?> RemoveStockAsync(int productId, UpdateStockDto stockDto)
    {
        if (stockDto.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be positive when removing stock");
        }

        var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
        if (inventory == null) return null;

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) return null;

        // Check if we have enough stock
        if (inventory.CurrentStock < stockDto.Quantity)
        {
            throw new InvalidOperationException("Insufficient stock available");
        }

        // Remove stock
        inventory.CurrentStock -= stockDto.Quantity;
        inventory.LastUpdated = DateTime.UtcNow;

        // Update inventory
        await _inventoryRepository.UpdateAsync(inventory);

        return new InventoryDto(
            productId,
            product.Name,
            product.SKU,
            inventory.CurrentStock
        );
    }
} 