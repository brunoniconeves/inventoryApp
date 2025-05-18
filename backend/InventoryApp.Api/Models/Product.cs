using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Api.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = null!;

    [Required]
    public decimal Price { get; set; }

    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ConcurrencyCheck]
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public Inventory? Inventory { get; set; }
} 