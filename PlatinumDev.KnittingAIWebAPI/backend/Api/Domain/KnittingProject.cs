using System;

using PlatinumDev.KnittingAIWebAPI.Domain.Schemes;

namespace PlatinumDev.KnittingAIWebAPI.Domain;

/// <summary>
/// Projekt robótki na drutach: nazwa + różne diagramy.
/// </summary>
public class KnittingProject
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; set; } = default!;

    public string ImageUrl { get; set; } = string.Empty;

    public TextKnittingScheme? TextScheme { get; set; }

    public SymbolicKnittingScheme? SymbolicScheme { get; set; }

    public PixelKnittingScheme? PixelScheme { get; set; }

    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
}
