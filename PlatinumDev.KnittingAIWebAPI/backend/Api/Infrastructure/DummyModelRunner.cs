using PlatinumDev.KnittingAIWebAPI.Domain;

namespace PlatinumDev.KnittingAIWebAPI.Infrastructure;

/// <summary>
/// Заглушка для локальной разработки без запущенного FastAPI.
/// </summary>
public class DummyModelRunner : IModelRunner
{
    public ModelOutputData RunModel(Stream image)
    {
        // Можно прочитать поток, если нужно (например, проверка размера).
        // Здесь возвращаем фиктивные данные.
        return new ModelOutputData(
            OutputPath: "/tmp/fake-output.png",
            Confidence: 0.987,
            Extra: new Dictionary<string, object>
            {
                ["note"] = "Dummy response (no FastAPI)",
                ["ts"] = DateTime.UtcNow.ToString("O")
            }
        );
    }
}
