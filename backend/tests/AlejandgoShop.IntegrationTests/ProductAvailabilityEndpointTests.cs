using System.Net;
using System.Net.Http.Json;
using AlejandgoShop.Domain.Catalog;
using AlejandgoShop.Domain.Enums;
using AlejandgoShop.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace AlejandgoShop.IntegrationTests;

public class ProductAvailabilityEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProductAvailabilityEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Sin_diseño_la_disponibilidad_es_el_stock_de_la_variante()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var product = new Product("Camiseta Test", "desc", 19.99m, ProductCategory.Camiseta, allowsCustomization: true);
        product.AddVariant("Negro", Size.M, stock: 25, sku: $"TEST-{Guid.NewGuid()}");
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var variantId = product.Variants[0].Id;
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/products/{product.Id}/variants/{variantId}/availability");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AvailabilityResponse>();
        result!.AvailableQuantity.ShouldBe(25);

        db.Products.Remove(product);
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task Con_diseño_premium_la_disponibilidad_es_el_stock_del_diseño()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var product = new Product("Sudadera Test", "desc", 39.99m, ProductCategory.Sudadera, allowsCustomization: true);
        product.AddVariant("Blanco", Size.L, stock: 100, sku: $"TEST-{Guid.NewGuid()}");
        db.Products.Add(product);

        var design = new CatalogDesign("Diseño Premium Test", "https://example.com/d.png", isPremium: true, stock: 4);
        db.CatalogDesigns.Add(design);

        await db.SaveChangesAsync();

        var variantId = product.Variants[0].Id;
        var client = _factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/products/{product.Id}/variants/{variantId}/availability?catalogDesignId={design.Id}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AvailabilityResponse>();
        result!.AvailableQuantity.ShouldBe(4);

        db.Products.Remove(product);
        db.CatalogDesigns.Remove(design);
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task Producto_inexistente_devuelve_404()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/products/{Guid.NewGuid()}/variants/{Guid.NewGuid()}/availability");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private record AvailabilityResponse(Guid ProductId, Guid VariantId, Guid? CatalogDesignId, int AvailableQuantity);
}