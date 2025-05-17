using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InventoryApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "integer", nullable: false),
                    SKU = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Price", "SKU", "StockQuantity", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470), "High-precision optical sensor with 16000 DPI, ergonomic design, and RGB lighting", "Professional Gaming Mouse", 79.99m, "MOUSE-PRO-001", 50, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470) },
                    { 2, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470), "Cherry MX Blue switches, full RGB backlight, aluminum frame", "Mechanical Keyboard", 129.99m, "KB-MECH-002", 30, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470) },
                    { 3, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470), "27-inch 4K IPS display, 144Hz refresh rate, 1ms response time", "4K Gaming Monitor", 499.99m, "MON-4K-003", 15, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470) },
                    { 4, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470), "7.1 surround sound, noise-canceling mic, memory foam ear cushions", "Gaming Headset", 89.99m, "HEAD-PRO-004", 40, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470) },
                    { 5, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470), "Extended size mousepad with RGB border, anti-slip base", "RGB Mousepad XL", 29.99m, "PAD-RGB-005", 60, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470) },
                    { 6, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470), "Ergonomic design, adjustable armrests, lumbar support, PU leather", "Gaming Chair", 249.99m, "CHAIR-PRO-006", 20, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470) },
                    { 7, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470), "Professional condenser microphone, plug & play, cardioid pattern", "USB Microphone", 119.99m, "MIC-USB-007", 25, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470) },
                    { 8, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470), "8GB GDDR6, ray tracing, DLSS 3.0 support", "Graphics Card RTX 4060", 399.99m, "GPU-4060-008", 10, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470) },
                    { 9, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470), "Dual-band WiFi 6, gaming prioritization, 4 ethernet ports", "Gaming Router", 179.99m, "NET-GAME-009", 35, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470) },
                    { 10, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470), "15 LCD keys, customizable actions, live content creation control", "Streaming Deck", 149.99m, "STREAM-PRO-010", 15, new DateTime(2025, 5, 17, 15, 23, 59, 39, DateTimeKind.Utc).AddTicks(470) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
