namespace PlatinumDev.KnittingAIWebAPI.Domain.Schemes;

/// <summary>
/// Schemat pikseli/rastrów (podgląd, eksport PNG, itd.).
/// </summary>
public class PixelKnittingScheme : IScheme
{
    public byte[]? PngBytes { get; set; }
}
