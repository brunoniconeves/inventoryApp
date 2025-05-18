using Microsoft.EntityFrameworkCore;
using InventoryApp.Api.Data;
using InventoryApp.Api.Models;

namespace InventoryApp.Api.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Inventory?> GetByProductIdAsync(int productId)
    {
        return await _context.Inventory
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.ProductId == productId);
    }

    public async Task<Inventory> UpdateAsync(Inventory inventory)
    {
        _context.Entry(inventory).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return inventory;
    }

    public async Task<IEnumerable<Inventory>> GetAllAsync()
    {
        return await _context.Inventory.ToListAsync();
    }

    public async Task<Inventory> AddAsync(Inventory inventory)
    {
        _context.Inventory.Add(inventory);
        await _context.SaveChangesAsync();
        return inventory;
    }
} 