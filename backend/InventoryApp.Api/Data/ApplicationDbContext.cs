using Microsoft.EntityFrameworkCore;
using InventoryApp.Api.Models;

namespace InventoryApp.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Inventory> Inventory { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Product configuration
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Product>()
            .Property(p => p.SKU)
            .HasMaxLength(50);

        modelBuilder.Entity<Product>()
            .Property(p => p.Name)
            .HasMaxLength(200);

        modelBuilder.Entity<Product>()
            .Property(p => p.Description)
            .HasMaxLength(2000);

        // Inventory configuration
        modelBuilder.Entity<Inventory>()
            .HasOne(i => i.Product)
            .WithOne(p => p.Inventory)
            .HasForeignKey<Inventory>(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed Data
        var now = DateTime.UtcNow;
        
        // Seed products
        var products = new[]
        {
            new Product
            {
                Id = 1,
                Name = "Professional Gaming Mouse",
                Description = "High-precision optical sensor with 16000 DPI, ergonomic design, and RGB lighting",
                Price = 79.99m,
                SKU = "MOUSE-PRO-001",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Product
            {
                Id = 2,
                Name = "Mechanical Keyboard",
                Description = "Cherry MX Blue switches, full RGB backlight, aluminum frame",
                Price = 129.99m,
                SKU = "KB-MECH-002",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Product
            {
                Id = 3,
                Name = "4K Gaming Monitor",
                Description = "27-inch 4K IPS display, 144Hz refresh rate, 1ms response time",
                Price = 499.99m,
                SKU = "MON-4K-003",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Product
            {
                Id = 4,
                Name = "Gaming Headset",
                Description = "7.1 surround sound, noise-canceling mic, memory foam ear cushions",
                Price = 89.99m,
                SKU = "HEAD-PRO-004",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Product
            {
                Id = 5,
                Name = "RGB Mousepad XL",
                Description = "Extended size mousepad with RGB border, anti-slip base",
                Price = 29.99m,
                SKU = "PAD-RGB-005",
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        modelBuilder.Entity<Product>().HasData(products);

        // Seed inventory for products
        modelBuilder.Entity<Inventory>().HasData(
            new Inventory
            {
                Id = 1,
                ProductId = 1,
                CurrentStock = 50,
                LastUpdated = now
            },
            new Inventory
            {
                Id = 2,
                ProductId = 2,
                CurrentStock = 30,
                LastUpdated = now
            },
            new Inventory
            {
                Id = 3,
                ProductId = 3,
                CurrentStock = 15,
                LastUpdated = now
            },
            new Inventory
            {
                Id = 4,
                ProductId = 4,
                CurrentStock = 40,
                LastUpdated = now
            },
            new Inventory
            {
                Id = 5,
                ProductId = 5,
                CurrentStock = 60,
                LastUpdated = now
            }
        );
    }
} 