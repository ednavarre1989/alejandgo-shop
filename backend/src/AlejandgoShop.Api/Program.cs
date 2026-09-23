using AlejandgoShop.Domain.Catalog;
using AlejandgoShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/products", async (AppDbContext db) =>
{
    var products = await db.Products
        .Include(p => p.Variants)
        .ToListAsync();
    return Results.Ok(products);
})
.WithName("GetProducts");

app.MapGet("/api/product-variants", async (AppDbContext db) =>
{
    var variants = await db.ProductVariants.ToListAsync();
    return Results.Ok(variants);
})
.WithName("GetProductVariants");

app.MapGet("/api/catalog-designs", async (AppDbContext db) =>
{
    var designs = await db.CatalogDesigns.ToListAsync();
    return Results.Ok(designs);
})
.WithName("GetCatalogDesigns");

app.MapGet("/api/products/{productId:guid}/variants/{variantId:guid}/availability",
    async (Guid productId, Guid variantId, Guid? catalogDesignId, AppDbContext db) =>
    {
        var product = await db.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product is null)
        {
            return Results.NotFound();
        }

        var variant = product.Variants.FirstOrDefault(v => v.Id == variantId);
        if (variant is null)
        {
            return Results.NotFound();
        }

        CatalogDesign? catalogDesign = null;
        if (catalogDesignId is not null)
        {
            catalogDesign = await db.CatalogDesigns.FindAsync(catalogDesignId.Value);
            if (catalogDesign is null)
            {
                return Results.NotFound();
            }
        }

        var availableQuantity = StockAvailabilityPolicy.GetAvailableQuantity(variant, catalogDesign);

        return Results.Ok(new
        {
            ProductId = productId,
            VariantId = variantId,
            CatalogDesignId = catalogDesignId,
            AvailableQuantity = availableQuantity
        });
    })
.WithName("GetProductVariantAvailability");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
