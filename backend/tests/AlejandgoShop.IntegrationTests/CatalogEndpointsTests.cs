using System.Net;
using AlejandgoShop.Domain.Catalog;
using AlejandgoShop.Domain.Enums;
using AlejandgoShop.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace AlejandgoShop.IntegrationTests;

public class CatalogEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CatalogEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetProductVariants_devuelve_200()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/product-variants");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCatalogDesigns_devuelve_200()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/catalog-designs");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Se_puede_guardar_y_leer_un_ProductVariant_de_SQL_Server_real()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var sku = $"TEST-{Guid.NewGuid()}";
        db.ProductVariants.Add(new ProductVariant("Rojo", Size.L, stock: 15, sku: sku));
        await db.SaveChangesAsync();

        var saved = await db.ProductVariants.FirstOrDefaultAsync(v => v.Sku == sku);

        saved.ShouldNotBeNull();
        saved!.Stock.ShouldBe(15);

        // limpieza, para no dejar basura en la BD de desarrollo
        db.ProductVariants.Remove(saved);
        await db.SaveChangesAsync();
    }
}