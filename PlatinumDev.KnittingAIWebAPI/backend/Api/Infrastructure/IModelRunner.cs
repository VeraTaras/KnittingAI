using System.IO;
using PlatinumDev.KnittingAIWebAPI.Domain;

namespace PlatinumDev.KnittingAIWebAPI.Infrastructure;

/// <summary>
/// Kontrakt uruchamiania modelu AI.
/// Implementacje infrastrukturalne (HTTP/Docker itp.) muszą go zaimplementować.
/// </summary>
public interface IModelRunner
{
    /// <summary>
    /// Uruchamia model, przekazując strumień obrazu, i zwraca dane wyniku.
    /// </summary>
    ModelOutputData RunModel(Stream image);
}
