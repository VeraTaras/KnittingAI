namespace PlatinumDev.KnittingAIWebAPI.Domain.Schemes;

/// <summary>
/// Текстовая схема/инструкция по вязанию.
/// </summary>
public class TextKnittingScheme : IScheme
{
    public string? Content { get; set; }
}
