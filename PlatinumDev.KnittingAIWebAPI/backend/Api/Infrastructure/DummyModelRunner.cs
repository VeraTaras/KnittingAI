using PlatinumDev.KnittingAIWebAPI.Domain;

namespace PlatinumDev.KnittingAIWebAPI.Infrastructure;

/// <summary>
/// Atrapa do lokalnego developmentu bez uruchomionego FastAPI.
/// </summary>
public class DummyModelRunner : IModelRunner
{
    public ModelOutputData RunModel(Stream image)
    {
        // Można odczytać strumień, jeśli potrzebne (np. sprawdzenie rozmiaru).
        // Tutaj zwracamy fikcyjne dane.
        return new ModelOutputData(
            OutputPath: "/tmp/fake-output.png",
            Confidence: 0.987,
            Extra: new Dictionary<string, object>
            {
                ["note"] = "Dummy response (brak FastAPI)",
                ["ts"] = DateTime.UtcNow.ToString("O")
            }
        );
    }
}
