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
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SKU = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    CurrentStock = table.Column<int>(type: "integer", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Price", "SKU", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311), "High-precision optical sensor with 16000 DPI, ergonomic design, and RGB lighting", "Professional Gaming Mouse", 79.99m, "MOUSE-PRO-001", new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311) },
                    { 2, new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311), "Cherry MX Blue switches, full RGB backlight, aluminum frame", "Mechanical Keyboard", 129.99m, "KB-MECH-002", new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311) },
                    { 3, new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311), "27-inch 4K IPS display, 144Hz refresh rate, 1ms response time", "4K Gaming Monitor", 499.99m, "MON-4K-003", new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311) },
                    { 4, new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311), "7.1 surround sound, noise-canceling mic, memory foam ear cushions", "Gaming Headset", 89.99m, "HEAD-PRO-004", new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311) },
                    { 5, new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311), "Extended size mousepad with RGB border, anti-slip base", "RGB Mousepad XL", 29.99m, "PAD-RGB-005", new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311) }
                });

            migrationBuilder.InsertData(
                table: "Inventory",
                columns: new[] { "Id", "CurrentStock", "LastUpdated", "ProductId" },
                values: new object[,]
                {
                    { 1, 50, new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311), 1 },
                    { 2, 30, new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311), 2 },
                    { 3, 15, new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311), 3 },
                    { 4, 40, new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311), 4 },
                    { 5, 60, new DateTime(2025, 5, 18, 0, 19, 59, 80, DateTimeKind.Utc).AddTicks(8311), 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ProductId",
                table: "Inventory",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
