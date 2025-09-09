namespace PlatinumDev.KnittingAIWebAPI.Domain.Schemes;

/// <summary>
/// Пиксельная/растровая схема (превью, экспорт в PNG и т.п.).
/// </summary>
public class PixelKnittingScheme : IScheme
{
    public byte[]? PngBytes { get; set; }
}
