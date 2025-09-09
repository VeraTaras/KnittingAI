using System.Net.Http.Headers;
using System.Net.Http.Json;
using PlatinumDev.KnittingAIWebAPI.Domain;

namespace PlatinumDev.KnittingAIWebAPI.Infrastructure;

/// <summary>
/// Реализация IModelRunner, вызывающая внешний AI‑сервис (FastAPI) по HTTP.
/// Ожидается эндпоинт: POST /infer (multipart/form-data)
/// Ответ (пример):
/// { "output_path": "/app/output/input-123.png", "confidence": 0.93, "extra": { ... } }
/// </summary>
public class HttpModelRunner : IModelRunner
{
    private readonly IHttpClientFactory _httpFactory;

    public HttpModelRunner(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    public ModelOutputData RunModel(Stream image)
    {
        var client = _httpFactory.CreateClient("ai");

        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(image);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content.Add(fileContent, "file", "upload.jpg");

        // Выполняем запрос синхронно (для простоты MVP).
        // В продакшене стоит сделать интерфейс асинхронным.
        var response = client.PostAsync("/infer", content).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();

        var payload = response.Content.ReadFromJsonAsync<InferResponse>()
            .GetAwaiter().GetResult()
            ?? throw new InvalidOperationException("Empty model response");

        return new ModelOutputData(
            OutputPath: payload.output_path,
            Confidence: payload.confidence,
            Extra:      payload.extra
        );
    }

    private sealed class InferResponse
    {
        public string output_path { get; set; } = default!;
        public double? confidence { get; set; }
        public Dictionary<string, object>? extra { get; set; }
    }
}
