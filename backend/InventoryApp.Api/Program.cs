using Microsoft.EntityFrameworkCore;
using InventoryApp.Api.Data;
using InventoryApp.Api.Models;
using InventoryApp.Api.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// API Endpoints
app.MapGet("/api/products", async (ApplicationDbContext db) =>
{
    var products = await db.Products.ToListAsync();
    return Results.Ok(products.Select(p => new ProductDto(
        p.Id, p.Name, p.Description, p.Price, 
        p.StockQuantity, p.SKU, p.CreatedAt, p.UpdatedAt)));
})
.WithName("GetProducts")
.WithOpenApi();

app.MapGet("/api/products/{id}", async (int id, ApplicationDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null)
        return Results.NotFound();

    return Results.Ok(new ProductDto(
        product.Id, product.Name, product.Description, product.Price,
        product.StockQuantity, product.SKU, product.CreatedAt, product.UpdatedAt));
})
.WithName("GetProduct")
.WithOpenApi();

app.MapPost("/api/products", async (CreateProductDto productDto, ApplicationDbContext db) =>
{
    var product = new Product
    {
        Name = productDto.Name,
        Description = productDto.Description,
        Price = productDto.Price,
        StockQuantity = productDto.StockQuantity,
        SKU = productDto.SKU,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created($"/api/products/{product.Id}", new ProductDto(
        product.Id, product.Name, product.Description, product.Price,
        product.StockQuantity, product.SKU, product.CreatedAt, product.UpdatedAt));
})
.WithName("CreateProduct")
.WithOpenApi();

app.MapPut("/api/products/{id}", async (int id, UpdateProductDto productDto, ApplicationDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null)
        return Results.NotFound();

    product.Name = productDto.Name;
    product.Description = productDto.Description;
    product.Price = productDto.Price;
    product.SKU = productDto.SKU;
    product.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();

    return Results.Ok(new ProductDto(
        product.Id, product.Name, product.Description, product.Price,
        product.StockQuantity, product.SKU, product.CreatedAt, product.UpdatedAt));
})
.WithName("UpdateProduct")
.WithOpenApi();

app.MapDelete("/api/products/{id}", async (int id, ApplicationDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null)
        return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("DeleteProduct")
.WithOpenApi();

app.MapPost("/api/products/{id}/stock", async (int id, UpdateStockDto stockDto, ApplicationDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null)
        return Results.NotFound();

    product.StockQuantity += stockDto.Quantity;
    product.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();

    return Results.Ok(new ProductDto(
        product.Id, product.Name, product.Description, product.Price,
        product.StockQuantity, product.SKU, product.CreatedAt, product.UpdatedAt));
})
.WithName("AddStock")
.WithOpenApi();

app.MapDelete("/api/products/{id}/stock", async (int id, UpdateStockDto stockDto, ApplicationDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null)
        return Results.NotFound();

    if (product.StockQuantity < stockDto.Quantity)
        return Results.BadRequest("Not enough stock available");

    product.StockQuantity -= stockDto.Quantity;
    product.UpdatedAt = DateTime.UtcNow;

    await db.SaveChangesAsync();

    return Results.Ok(new ProductDto(
        product.Id, product.Name, product.Description, product.Price,
        product.StockQuantity, product.SKU, product.CreatedAt, product.UpdatedAt));
})
.WithName("RemoveStock")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
