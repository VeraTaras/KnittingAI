namespace PlatinumDev.KnittingAIWebAPI.Domain.Schemes;

/// <summary>
/// Schemat tekstowy/instrukcja robienia na drutach.
/// </summary>
public class TextKnittingScheme : IScheme
{
    public string? Content { get; set; }
}
