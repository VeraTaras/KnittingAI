using PlatinumDev.KnittingAIWebAPI.backend.Api;
using PlatinumDev.KnittingAIWebAPI.Infrastructure;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var outputPath = Path.Combine("/tmp"); // теперь /tmp общая для ML и backend

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient для FastAPI (если нужен HttpModelRunner)
builder.Services.AddHttpClient("ai", client =>
{
    var baseUrl = builder.Configuration["ML_SERVER_URL"] ?? "http://mlserver:8000";
    client.BaseAddress = new Uri(baseUrl);
});

// DI: все интерфейсы из Infrastructure
builder.Services.AddSingleton<IProjectRepository, InMemoryProjectRepository>();

// для разработки без FastAPI:
// builder.Services.AddScoped<IModelRunner, DummyModelRunner>();

// для реальной интеграции (заменишь на):
builder.Services.AddScoped<IModelRunner, HttpModelRunner>();

// фасада из Domain, но она зависит от интерфейсов Infrastructure
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

// Раздаём статику
app.UseStaticFiles();

app.MapProjectEndpoints();

app.Run();


