using AlejandgoShop.Domain.Catalog;
using AlejandgoShop.Domain.Enums;
using AlejandgoShop.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using SkiaSharp;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace AlejandgoShop.IntegrationTests;

public class CustomDesignUploadEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CustomDesignUploadEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Una_imagen_de_alta_resolucion_es_valida()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var product = new Product("Tapete Test", "desc", 25m, ProductCategory.Otro,
            allowsCustomization: true, customDesignDimensions: PrintDimensions.TcgPlaymat);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var imageBytes = CreateTestImage(width: 7200, height: 4200);
        var client = _factory.CreateClient();

        using var content = new MultipartFormDataContent();
        using var imageContent = new ByteArrayContent(imageBytes);
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        content.Add(imageContent, "file", "design.png");

        var response = await client.PostAsync($"/api/products/{product.Id}/custom-designs/validate", content);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ValidationResponse>();
        result!.IsValid.ShouldBeTrue();

        db.Products.Remove(product);
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task Una_imagen_de_baja_resolucion_no_es_valida()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var product = new Product("Tapete Test 2", "desc", 25m, ProductCategory.Otro,
            allowsCustomization: true, customDesignDimensions: PrintDimensions.TcgPlaymat);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var imageBytes = CreateTestImage(width: 800, height: 600);
        var client = _factory.CreateClient();

        using var content = new MultipartFormDataContent();
        using var imageContent = new ByteArrayContent(imageBytes);
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        content.Add(imageContent, "file", "design.png");

        var response = await client.PostAsync($"/api/products/{product.Id}/custom-designs/validate", content);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ValidationResponse>();
        result!.IsValid.ShouldBeFalse();

        db.Products.Remove(product);
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task Un_producto_que_no_permite_personalizacion_devuelve_400()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var product = new Product("Gorra Test", "desc", 12m, ProductCategory.Gorra, allowsCustomization: false);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var imageBytes = CreateTestImage(width: 7200, height: 4200);
        var client = _factory.CreateClient();

        using var content = new MultipartFormDataContent();
        using var imageContent = new ByteArrayContent(imageBytes);
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        content.Add(imageContent, "file", "design.png");

        var response = await client.PostAsync($"/api/products/{product.Id}/custom-designs/validate", content);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        db.Products.Remove(product);
        await db.SaveChangesAsync();
    }

    private static byte[] CreateTestImage(int width, int height)
    {
        using var bitmap = new SKBitmap(width, height);
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private record ValidationResponse(bool IsValid, int ImageWidthPx, int ImageHeightPx);
}