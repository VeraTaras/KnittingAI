namespace PlatinumDev.KnittingAIWebAPI.Domain.Schemes;

/// <summary>
/// Символьная (условные обозначения) схема. Удобно хранить как SVG.
/// </summary>
public class SymbolicKnittingScheme : IScheme
{
    /// <summary>
    /// Содержимое SVG (или другой векторный формат).
    /// </summary>
    public string? SvgData { get; set; }
}
