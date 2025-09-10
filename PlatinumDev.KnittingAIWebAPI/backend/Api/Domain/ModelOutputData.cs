using System.Collections.Generic;

namespace PlatinumDev.KnittingAIWebAPI.Domain;

/// <summary>
/// Dane zwracane przez serwis AI/model po inferencji.
/// </summary>
public record ModelOutputData(
    string OutputPath,
    double? Confidence = null,
    Dictionary<string, object>? Extra = null
);
