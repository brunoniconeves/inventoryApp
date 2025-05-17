using InventoryApp.Api.DTOs;

namespace InventoryApp.Api.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductDto productDto);
    Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto productDto);
    Task<bool> DeleteProductAsync(int id);
    Task<ProductDto?> AddStockAsync(int id, UpdateStockDto stockDto);
    Task<ProductDto?> RemoveStockAsync(int id, UpdateStockDto stockDto);
} 