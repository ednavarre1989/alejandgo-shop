using System.Net;
using System.Net.Http.Json;
using AlejandgoShop.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace AlejandgoShop.IntegrationTests;

public class CreateProductEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CreateProductEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Crear_un_producto_sin_personalizacion_devuelve_201()
    {
        var client = _factory.CreateClient();
        var request = new CreateProductRequest(
            Name: "Camiseta Test",
            Description: "desc",
            BasePrice: 19.99m,
            Category: "Camiseta",
            AllowsCustomization: false,
            CustomDesignDimensions: null);

        var response = await client.PostAsJsonAsync("/api/products", request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<CreatedProductResponse>();
        created!.Name.ShouldBe("Camiseta Test");
        created.Id.ShouldNotBe(Guid.Empty);

        await CleanUp(created.Id);
    }

    [Fact]
    public async Task Crear_un_producto_personalizable_requiere_dimensiones()
    {
        var client = _factory.CreateClient();
        var request = new CreateProductRequest(
            Name: "Tapete sin dimensiones",
            Description: "desc",
            BasePrice: 25m,
            Category: "Otro",
            AllowsCustomization: true,
            CustomDesignDimensions: null);

        var response = await client.PostAsJsonAsync("/api/products", request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Crear_un_producto_personalizable_con_dimensiones_devuelve_201()
    {
        var client = _factory.CreateClient();
        var request = new CreateProductRequest(
            Name: "Tapete TCG Test",
            Description: "desc",
            BasePrice: 25m,
            Category: "Otro",
            AllowsCustomization: true,
            CustomDesignDimensions: "TcgPlaymat");

        var response = await client.PostAsJsonAsync("/api/products", request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<CreatedProductResponse>();

        await CleanUp(created!.Id);
    }

    private async Task CleanUp(Guid productId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product = await db.Products.FindAsync(productId);
        if (product is not null)
        {
            db.Products.Remove(product);
            await db.SaveChangesAsync();
        }
    }

    private record CreateProductRequest(
        string Name,
        string Description,
        decimal BasePrice,
        string Category,
        bool AllowsCustomization,
        string? CustomDesignDimensions);

    private record CreatedProductResponse(Guid Id, string Name);
}