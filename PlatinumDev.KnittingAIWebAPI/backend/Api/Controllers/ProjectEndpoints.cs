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
                    logger.LogInformation("📥 New project request at {time}", DateTime.UtcNow);

                    if (file is null || file.Length == 0)
                    {
                        logger.LogWarning("⚠️ No file received!");
                        return Results.BadRequest("Missing file");
                    }

                    logger.LogInformation("✅ File {filename} received, size {size} bytes", file.FileName, file.Length);

                    name ??= $"Project-{DateTime.UtcNow:yyyyMMdd-HHmmss}";

                    await using var stream = file.OpenReadStream();
                    var modelData = facade.AnalyzeImage(stream);
                    logger.LogInformation("🧠 Model analyzed");

                    var schemes = facade.GenerateSchemes(modelData);
                    logger.LogInformation("🪡 Schemes generated: {count}", schemes.Count);

                    var project = facade.AssembleProject(schemes, name);
                    facade.SaveProject(project);
                    logger.LogInformation("💾 Project saved with id {id}", project.Id);

                    // --- Ищем PNG в общей папке ---
                    var sharedDir = "/shared_output";
                    if (!Directory.Exists(sharedDir))
                    {
                        logger.LogError("❌ Shared output folder not found: {path}", sharedDir);
                        return Results.BadRequest("Output folder not found");
                    }

                    var pngFiles = Directory.GetFiles(sharedDir, "*.png", SearchOption.AllDirectories);
                    if (pngFiles.Length == 0)
                    {
                        logger.LogError("❌ No PNG files found in {path}", sharedDir);
                        return Results.BadRequest("No output image found");
                    }

                    var latestPng = pngFiles
                        .Select(f => new FileInfo(f))
                        .OrderByDescending(f => f.CreationTimeUtc)
                        .First();

                    logger.LogInformation("🖼️ Found PNG: {file}", latestPng.FullName);

                    // --- Копируем в wwwroot/results/{projectId}/ ---
                    var resultsDir = Path.Combine(env.WebRootPath ?? "./wwwroot", "results", project.Id.ToString());
                    Directory.CreateDirectory(resultsDir);

                    var finalFile = Path.Combine(resultsDir, latestPng.Name);
                    File.Copy(latestPng.FullName, finalFile, true);

                    var imageUrl = $"/results/{project.Id}/{latestPng.Name}";
                    logger.LogInformation("🔗 Image available at {url}", imageUrl);

                    return Results.Created($"/projects/{project.Id}", new ProjectCreated(project.Id, imageUrl));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "❌ Error creating project");
                    return Results.Problem("Internal server error: " + ex.Message);
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
