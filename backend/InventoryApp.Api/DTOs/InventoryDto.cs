namespace InventoryApp.Api.DTOs;

public record InventoryDto(
    int ProductId,
    string ProductName,
    string SKU,
    int CurrentStock
);

public record UpdateStockDto(
    int Quantity
); 