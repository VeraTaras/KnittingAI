using PlatinumDev.KnittingAIWebAPI.backend.Api;
using PlatinumDev.KnittingAIWebAPI.Infrastructure;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var outputPath = Path.Combine("/tmp"); // teraz /tmp wspólny dla ML i backendu

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient dla FastAPI (jeśli potrzebny HttpModelRunner)
builder.Services.AddHttpClient("ai", client =>
{
    var baseUrl = builder.Configuration["ML_SERVER_URL"] ?? "http://mlserver:8000";
    client.BaseAddress = new Uri(baseUrl);
});

// DI: wszystkie interfejsy z Infrastructure
builder.Services.AddSingleton<IProjectRepository, InMemoryProjectRepository>();

// dla developmentu bez FastAPI:
// builder.Services.AddScoped<IModelRunner, DummyModelRunner>();

// dla realnej integracji (zamienisz na):
builder.Services.AddScoped<IModelRunner, HttpModelRunner>();

// fasada z Domain, ale zależna od interfejsów z Infrastructure
builder.Services.AddScoped<PlatinumDev.KnittingAIWebAPI.Domain.KnittingProcessorFacade>();

var app = builder.Build();

if (!Directory.Exists(app.Environment.WebRootPath))
{
    Directory.CreateDirectory(app.Environment.WebRootPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(app.Environment.WebRootPath),
    RequestPath = ""
});

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Knitting MVP API (.NET 8, Minimal APIs)");
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// Serwujemy pliki statyczne
app.UseStaticFiles();

app.MapProjectEndpoints();

app.Run();
