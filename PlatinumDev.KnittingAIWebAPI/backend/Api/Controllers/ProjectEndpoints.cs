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

                    name ??= $"Projekt {DateTime.UtcNow:HHmmss}";

                    await using var stream = file.OpenReadStream();
                    logger.LogInformation("➡️ Rozpoczynam analizę obrazu przez ML");
                    var modelData = facade.AnalyzeImage(stream);
                    logger.LogInformation("🧠 Model przeanalizowany");

                    logger.LogInformation("➡️ Generuję schematy na podstawie modelu");
                    var schemes = facade.GenerateSchemes(modelData);
                    logger.LogInformation("🪡 Schematy wygenerowane: {count}", schemes.Count);

                    // --- Szukamy PNG w wspólnym folderze ---
                    var sharedDir = "/shared_output";
                    logger.LogInformation("📂 Szukam plików PNG w {dir}", sharedDir);

                    if (!Directory.Exists(sharedDir))
                    {
                        logger.LogError("❌ Nie znaleziono wspólnego folderu wyjściowego: {path}", sharedDir);
                        return Results.BadRequest("Nie znaleziono folderu wyjściowego");
                    }

                    var pngFiles = Directory.GetFiles(sharedDir, "*.png", SearchOption.AllDirectories);
                    logger.LogInformation("📊 Liczba znalezionych plików PNG: {count}", pngFiles.Length);

                    if (pngFiles.Length == 0)
                    {
                        logger.LogError("❌ Brak plików PNG w {path}", sharedDir);
                        return Results.BadRequest("Nie znaleziono obrazu wyjściowego");
                    }

                    var latestPng = pngFiles
                        .Select(f => new FileInfo(f))
                        .OrderByDescending(f => f.CreationTimeUtc)
                        .First();

                    logger.LogInformation("🖼️ Wybrano najnowszy PNG: {file}", latestPng.FullName);

                    logger.LogInformation("➡️ Składam projekt");
                    var project = facade.AssembleProject(schemes, name);

                    // --- Kopiujemy do wwwroot/results/{projectId}/ ---
                    var resultsDir = Path.Combine(env.WebRootPath ?? "./wwwroot", "results", project.Id.ToString());
                    logger.LogInformation("📂 Tworzę katalog wyników: {dir}", resultsDir);
                    Directory.CreateDirectory(resultsDir);

                    var finalFile = Path.Combine(resultsDir, latestPng.Name);
                    File.Copy(latestPng.FullName, finalFile, true);
                    logger.LogInformation("✅ Skopiowano plik do: {finalFile}", finalFile);

                    var imageUrl = $"/results/{project.Id}/{latestPng.Name}";
                    logger.LogInformation("🔗 Obraz dostępny pod adresem: {url}", imageUrl);
                    
                    project.ImageUrl = imageUrl;
                    facade.SaveProject(project);
                    logger.LogInformation("💾 Projekt zapisany z id {id}", project.Id);

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
    
        app.MapGet("/projects/{id:guid}", (Guid id, KnittingProcessorFacade facade, ILogger<Program> logger) =>
        {
            logger.LogInformation("📥 Żądanie pobrania projektu {id}", id);
            var project = facade.LoadProject(id);
            if (project is not null)
            {
                logger.LogInformation("✅ Projekt {id} znaleziony", id);
                return Results.Ok(project);
            }
            else
            {
                logger.LogWarning("⚠️ Projekt {id} nie został znaleziony", id);
                return Results.NotFound();
            }
        });

        app.MapGet("/projects", (KnittingProcessorFacade facade, ILogger<Program> logger) =>
        {
            logger.LogInformation("📥 Żądanie pobrania wszystkich projektów");
            var projects = facade.GetAllProjects();
            logger.LogInformation("📊 Znaleziono {count} projektów", projects.Count());
            return Results.Ok(projects);
        });
    }
}
