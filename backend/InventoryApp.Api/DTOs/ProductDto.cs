namespace InventoryApp.Api.DTOs;

public record ProductDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    string? SKU,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateProductDto(
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    string? SKU
);

public record UpdateProductDto(
    string Name,
    string? Description,
    decimal Price,
    string? SKU
);

public record UpdateStockDto(
    int Quantity
); 