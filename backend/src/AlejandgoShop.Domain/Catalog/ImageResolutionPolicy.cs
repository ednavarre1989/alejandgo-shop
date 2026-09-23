using AlejandgoShop.Domain.Enums;

namespace AlejandgoShop.Domain.Catalog;

/// <summary>
/// Valida que una imagen subida por el usuario cumpla la resolución mínima
/// exigida (300 ppp) para el área física de impresión del producto.
/// </summary>
public static class ImageResolutionPolicy
{
    private const double MinimumDpi = 300.0;
    private const double CmPerInch = 2.54;

    public static bool IsValidResolution(int imageWidthPx, int imageHeightPx, PrintDimensions dimensions)
    {
        var (widthCm, heightCm) = dimensions.GetSizeCm();

        var widthInches = widthCm / CmPerInch;
        var heightInches = heightCm / CmPerInch;

        var actualDpiWidth = imageWidthPx / widthInches;
        var actualDpiHeight = imageHeightPx / heightInches;

        return actualDpiWidth >= MinimumDpi && actualDpiHeight >= MinimumDpi;
    }
}