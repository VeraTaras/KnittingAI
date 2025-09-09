using System.Collections.Generic;

namespace PlatinumDev.KnittingAIWebAPI.Domain;

/// <summary>
/// Данные, возвращаемые сервисом AI/моделью после инференса.
/// </summary>
public record ModelOutputData(
    string OutputPath,
    double? Confidence = null,
    Dictionary<string, object>? Extra = null
);
