namespace AlejandgoShop.Domain.Enums;

public enum PrintDimensions
{
    TcgPlaymat,
    GamingMousepad
}

public static class PrintDimensionsExtensions
{
    public static (double WidthCm, double HeightCm) GetSizeCm(this PrintDimensions dimensions) => dimensions switch
    {
        PrintDimensions.TcgPlaymat => (60.0, 35.0),
        PrintDimensions.GamingMousepad => (90.0, 40.0),
        _ => throw new ArgumentOutOfRangeException(nameof(dimensions), dimensions, "Dimensiones de impresión no reconocidas.")
    };
}