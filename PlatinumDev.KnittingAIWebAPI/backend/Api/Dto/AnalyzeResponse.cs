using PlatinumDev.KnittingAIWebAPI.Domain;
using PlatinumDev.KnittingAIWebAPI.Domain.Schemes;

namespace PlatinumDev.KnittingAIWebAPI.Dto;

/// <summary>
/// Ответ на запрос анализа изображения.
/// Содержит «сырые» данные от модели и сгенерированные схемы.
/// </summary>
public record AnalyzeResponse(
    ModelOutputData ModelData,
    SymbolicKnittingScheme? Symbolic,
    TextKnittingScheme? Text
);
