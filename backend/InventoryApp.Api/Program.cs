using Microsoft.EntityFrameworkCore;
using InventoryApp.Api.Data;
using InventoryApp.Api.Services;
using InventoryApp.Api.Repositories;

namespace InventoryApp.Api;

public partial class Program 
{
    public static WebApplication CreateApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Kestrel
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenAnyIP(5000);
        });

        // Add services to the container.
        builder.Services.AddControllers();
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

        // Register services and repositories
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
        builder.Services.AddScoped<IInventoryService, InventoryService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors();
        app.MapControllers();

        return app;
    }

    public static void Main(string[] args)
    {
        var app = CreateApp(args);

        // Apply migrations at startup with retry mechanism
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            var db = services.GetRequiredService<ApplicationDbContext>();
            var env = services.GetRequiredService<IWebHostEnvironment>();

            // Skip migrations for testing environment
            if (!env.IsEnvironment("Testing"))
            {
                var retryCount = 0;
                const int maxRetries = 10;
                const int retryDelaySeconds = 5;

                while (retryCount < maxRetries)
                {
                    try
                    {
                        logger.LogInformation("Attempting to connect to the database and apply migrations...");
                        db.Database.Migrate();
                        logger.LogInformation("Database migrations applied successfully.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        if (retryCount >= maxRetries)
                        {
                            logger.LogError(ex, "Failed to connect to the database after {RetryCount} attempts.", maxRetries);
                            throw;
                        }

                        logger.LogWarning(
                            "Failed to connect to the database. Retrying in {RetryDelay} seconds... (Attempt {RetryCount} of {MaxRetries})",
                            retryDelaySeconds, retryCount, maxRetries);
                        
                        Thread.Sleep(TimeSpan.FromSeconds(retryDelaySeconds));
                    }
                }
            }
        }

        app.Run();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
