using AlejandgoShop.Domain.Catalog;
using AlejandgoShop.Domain.Enums;
using AlejandgoShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

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

app.MapPost("/api/products/{productId:guid}/custom-designs/validate",
    async (Guid productId, IFormFile file, AppDbContext db) =>
    {
        var product = await db.Products.FindAsync(productId);

        if (product is null)
        {
            return Results.NotFound();
        }

        if (!product.AllowsCustomization || product.CustomDesignDimensions is null)
        {
            return Results.BadRequest(new { error = "Este producto no admite diseños personalizados." });
        }

        await using var stream = file.OpenReadStream();
        using var codec = SKCodec.Create(stream);
        var width = codec.Info.Width;
        var height = codec.Info.Height;

        var isValid = ImageResolutionPolicy.IsValidResolution(width, height, product.CustomDesignDimensions.Value);

        return Results.Ok(new
        {
            IsValid = isValid,
            ImageWidthPx = width,
            ImageHeightPx = height
        });
    })
.WithName("ValidateCustomDesign")
.DisableAntiforgery();

app.MapPost("/api/products", async (CreateProductRequest request, AppDbContext db) =>
{
    ProductCategory category;
    if (!Enum.TryParse(request.Category, out category))
    {
        return Results.BadRequest(new { error = $"Categoría '{request.Category}' no reconocida." });
    }

    PrintDimensions? dimensions = null;
    if (request.CustomDesignDimensions is not null)
    {
        if (!Enum.TryParse<PrintDimensions>(request.CustomDesignDimensions, out var parsedDimensions))
        {
            return Results.BadRequest(new { error = $"Dimensiones '{request.CustomDesignDimensions}' no reconocidas." });
        }
        dimensions = parsedDimensions;
    }

    Product product;
    try
    {
        product = new Product(
            request.Name,
            request.Description,
            request.BasePrice,
            category,
            request.AllowsCustomization,
            dimensions);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }

    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created($"/api/products/{product.Id}", new { product.Id, product.Name });
})
.WithName("CreateProduct");

app.Run();

public partial class Program { }

record CreateProductRequest(
    string Name,
    string Description,
    decimal BasePrice,
    string Category,
    bool AllowsCustomization,
    string? CustomDesignDimensions);