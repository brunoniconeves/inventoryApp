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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        // Seed Data
        var now = DateTime.UtcNow;
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Professional Gaming Mouse",
                Description = "High-precision optical sensor with 16000 DPI, ergonomic design, and RGB lighting",
                Price = 79.99m,
                StockQuantity = 50,
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
                StockQuantity = 30,
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
                StockQuantity = 15,
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
                StockQuantity = 40,
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
                StockQuantity = 60,
                SKU = "PAD-RGB-005",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Product
            {
                Id = 6,
                Name = "Gaming Chair",
                Description = "Ergonomic design, adjustable armrests, lumbar support, PU leather",
                Price = 249.99m,
                StockQuantity = 20,
                SKU = "CHAIR-PRO-006",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Product
            {
                Id = 7,
                Name = "USB Microphone",
                Description = "Professional condenser microphone, plug & play, cardioid pattern",
                Price = 119.99m,
                StockQuantity = 25,
                SKU = "MIC-USB-007",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Product
            {
                Id = 8,
                Name = "Graphics Card RTX 4060",
                Description = "8GB GDDR6, ray tracing, DLSS 3.0 support",
                Price = 399.99m,
                StockQuantity = 10,
                SKU = "GPU-4060-008",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Product
            {
                Id = 9,
                Name = "Gaming Router",
                Description = "Dual-band WiFi 6, gaming prioritization, 4 ethernet ports",
                Price = 179.99m,
                StockQuantity = 35,
                SKU = "NET-GAME-009",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Product
            {
                Id = 10,
                Name = "Streaming Deck",
                Description = "15 LCD keys, customizable actions, live content creation control",
                Price = 149.99m,
                StockQuantity = 15,
                SKU = "STREAM-PRO-010",
                CreatedAt = now,
                UpdatedAt = now
            }
        );
    }
} 