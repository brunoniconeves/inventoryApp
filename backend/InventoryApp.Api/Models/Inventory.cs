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