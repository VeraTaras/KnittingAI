using PlatinumDev.KnittingAIWebAPI.Domain;
using PlatinumDev.KnittingAIWebAPI.Dto;
using Microsoft.Extensions.Logging;

namespace PlatinumDev.KnittingAIWebAPI.backend.Api;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this WebApplication app)
    {
        app.MapPost("/projects",
            async (IFormFile file,
                    string? name,
                    KnittingProcessorFacade facade,
                    IWebHostEnvironment env,
                    ILogger<Program> logger) =>
            {
                try
                {
                    logger.LogInformation("📥 Nowe żądanie projektu o {time}", DateTime.UtcNow);

                    if (file is null || file.Length == 0)
                    {
                        logger.LogWarning("⚠️ Nie otrzymano pliku!");
                        return Results.BadRequest("Brak pliku");
                    }

                    logger.LogInformation("✅ Plik {filename} otrzymany, rozmiar {size} bajtów", file.FileName, file.Length);

                    name ??= $"Project-{DateTime.UtcNow:yyyyMMdd-HHmmss}";

                    await using var stream = file.OpenReadStream();
                    var modelData = facade.AnalyzeImage(stream);
                    logger.LogInformation("🧠 Model przeanalizowany");

                    var schemes = facade.GenerateSchemes(modelData);
                    logger.LogInformation("🪡 Schematy wygenerowane: {count}", schemes.Count);

                    var project = facade.AssembleProject(schemes, name);
                    facade.SaveProject(project);
                    logger.LogInformation("💾 Projekt zapisany z id {id}", project.Id);

                    // --- Szukamy PNG w wspólnym folderze ---
                    var sharedDir = "/shared_output";
                    if (!Directory.Exists(sharedDir))
                    {
                        logger.LogError("❌ Nie znaleziono wspólnego folderu wyjściowego: {path}", sharedDir);
                        return Results.BadRequest("Nie znaleziono folderu wyjściowego");
                    }

                    var pngFiles = Directory.GetFiles(sharedDir, "*.png", SearchOption.AllDirectories);
                    if (pngFiles.Length == 0)
                    {
                        logger.LogError("❌ Brak plików PNG w {path}", sharedDir);
                        return Results.BadRequest("Nie znaleziono obrazu wyjściowego");
                    }

                    var latestPng = pngFiles
                        .Select(f => new FileInfo(f))
                        .OrderByDescending(f => f.CreationTimeUtc)
                        .First();

                    logger.LogInformation("🖼️ Znaleziono PNG: {file}", latestPng.FullName);

                    // --- Kopiujemy do wwwroot/results/{projectId}/ ---
                    var resultsDir = Path.Combine(env.WebRootPath ?? "./wwwroot", "results", project.Id.ToString());
                    Directory.CreateDirectory(resultsDir);

                    var finalFile = Path.Combine(resultsDir, latestPng.Name);
                    File.Copy(latestPng.FullName, finalFile, true);

                    var imageUrl = $"/results/{project.Id}/{latestPng.Name}";
                    logger.LogInformation("🔗 Obraz dostępny pod {url}", imageUrl);

                    return Results.Created($"/projects/{project.Id}", new ProjectCreated(project.Id, imageUrl));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "❌ Błąd tworzenia projektu");
                    return Results.Problem("Błąd serwera wewnętrznego: " + ex.Message);
                }
            })
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<ProjectCreated>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateProject")
            .WithTags("Projects")
            .DisableAntiforgery();
    }
}
