using System.Net.Http.Headers;
using System.Net.Http.Json;
using PlatinumDev.KnittingAIWebAPI.Domain;

namespace PlatinumDev.KnittingAIWebAPI.Infrastructure;

/// <summary>
/// Implementacja IModelRunner, która wywołuje zewnętrzny serwis AI (FastAPI) przez HTTP.
/// Oczekiwany endpoint: POST /infer (multipart/form-data)
/// Przykładowa odpowiedź:
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

        // Wykonujemy żądanie synchronicznie (dla prostoty MVP).
        // W produkcji warto zrobić interfejs asynchroniczny.
        var response = client.PostAsync("/infer", content).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();

        var payload = response.Content.ReadFromJsonAsync<InferResponse>()
            .GetAwaiter().GetResult()
            ?? throw new InvalidOperationException("Pusta odpowiedź modelu");

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
