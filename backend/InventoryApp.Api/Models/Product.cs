using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Api.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int StockQuantity { get; set; }

    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    [ConcurrencyCheck]
    public DateTime UpdatedAt { get; set; }
} 