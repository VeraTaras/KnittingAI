namespace PlatinumDev.KnittingAIWebAPI.Domain.Schemes;

/// <summary>
/// Schemat symboliczny (oznaczenia graficzne). Wygodnie przechowywać jako SVG.
/// </summary>
public class SymbolicKnittingScheme : IScheme
{
    /// <summary>
    /// Zawartość SVG (lub inny format wektorowy).
    /// </summary>
    public string? SvgData { get; set; }
}
