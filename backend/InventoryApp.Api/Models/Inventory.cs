using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Api.Models;

public class Inventory
{
    public int Id { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    public int CurrentStock { get; set; }
    
    [ConcurrencyCheck]
    public DateTime LastUpdated { get; set; }

    // Navigation property
    public Product Product { get; set; } = null!;
}

public class StockHistory
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string OperationType { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public DateTime Timestamp { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
    public Inventory Inventory { get; set; } = null!;
} 