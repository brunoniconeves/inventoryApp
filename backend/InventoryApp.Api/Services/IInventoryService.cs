using InventoryApp.Api.DTOs;

namespace InventoryApp.Api.Services;

public interface IInventoryService
{
    Task<InventoryDto?> GetProductStockAsync(int productId);
    Task<InventoryDto?> AddStockAsync(int productId, UpdateStockDto stockDto);
    Task<InventoryDto?> RemoveStockAsync(int productId, UpdateStockDto stockDto);
} 