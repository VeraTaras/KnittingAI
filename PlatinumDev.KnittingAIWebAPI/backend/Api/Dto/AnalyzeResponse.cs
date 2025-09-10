using PlatinumDev.KnittingAIWebAPI.Domain;
using PlatinumDev.KnittingAIWebAPI.Domain.Schemes;

namespace PlatinumDev.KnittingAIWebAPI.Dto;

/// <summary>
/// Odpowiedź na żądanie analizy obrazu.
/// Zawiera „surowe” dane z modelu oraz wygenerowane schematy.
/// </summary>
public record AnalyzeResponse(
    ModelOutputData ModelData,
    SymbolicKnittingScheme? Symbolic,
    TextKnittingScheme? Text
);
