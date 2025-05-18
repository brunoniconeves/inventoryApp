using InventoryApp.Api.Models;

namespace InventoryApp.Api.Repositories;

public interface IInventoryRepository
{
    Task<Inventory?> GetByProductIdAsync(int productId);
    Task<Inventory> UpdateAsync(Inventory inventory);
    Task<IEnumerable<Inventory>> GetAllAsync();
    Task<Inventory> AddAsync(Inventory inventory);
} 