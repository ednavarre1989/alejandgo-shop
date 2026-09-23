using AlejandgoShop.Domain.Catalog;
using AlejandgoShop.Domain.Enums;
using Shouldly;
using Xunit;

namespace AlejandgoShop.UnitTests.Domain;

public class ImageResolutionPolicyTests
{
    [Fact]
    public void Una_imagen_con_exactamente_300ppp_es_valida()
    {
        // TcgPlaymat: 60cm de ancho = 23.62 pulgadas. 300ppp * 23.62 ≈ 7087 px
        var isValid = ImageResolutionPolicy.IsValidResolution(
            imageWidthPx: 7087,
            imageHeightPx: 4134, // 35cm ≈ 13.78in * 300 ≈ 4134px
            dimensions: PrintDimensions.TcgPlaymat);

        isValid.ShouldBeTrue();
    }

    [Fact]
    public void Una_imagen_por_debajo_de_300ppp_no_es_valida()
    {
        var isValid = ImageResolutionPolicy.IsValidResolution(
            imageWidthPx: 1000,
            imageHeightPx: 800,
            dimensions: PrintDimensions.TcgPlaymat);

        isValid.ShouldBeFalse();
    }

    [Fact]
    public void Una_imagen_con_mas_de_300ppp_es_valida()
    {
        var isValid = ImageResolutionPolicy.IsValidResolution(
            imageWidthPx: 10000,
            imageHeightPx: 6000,
            dimensions: PrintDimensions.TcgPlaymat);

        isValid.ShouldBeTrue();
    }

    [Fact]
    public void Si_solo_una_dimension_cumple_300ppp_no_es_valida()
    {
        // ancho correcto, alto insuficiente
        var isValid = ImageResolutionPolicy.IsValidResolution(
            imageWidthPx: 7087,
            imageHeightPx: 1000,
            dimensions: PrintDimensions.TcgPlaymat);

        isValid.ShouldBeFalse();
    }

    [Fact]
    public void GamingMousepad_usa_sus_propias_medidas()
    {
        // 90cm ≈ 35.43in * 300 ≈ 10630px, 40cm ≈ 15.75in * 300 ≈ 4725px (redondeado hacia arriba)
        var isValid = ImageResolutionPolicy.IsValidResolution(
            imageWidthPx: 10630,
            imageHeightPx: 4725,
            dimensions: PrintDimensions.GamingMousepad);

        isValid.ShouldBeTrue();
    }
}