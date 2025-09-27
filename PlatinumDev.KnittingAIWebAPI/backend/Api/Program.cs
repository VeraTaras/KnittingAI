using PlatinumDev.KnittingAIWebAPI.backend.Api;
using PlatinumDev.KnittingAIWebAPI.Infrastructure;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Knitting AI API", Version = "v1" });
});

// --- HttpClient dla FastAPI (ML server) ---
builder.Services.AddHttpClient("ai", client =>
{
    var baseUrl = builder.Configuration["ML_SERVER_URL"] ?? "http://mlserver:8000";
    client.BaseAddress = new Uri(baseUrl);
});

// --- Dependency Injection ---
builder.Services.AddSingleton<IProjectRepository, InMemoryProjectRepository>();
builder.Services.AddScoped<IModelRunner, HttpModelRunner>();
builder.Services.AddScoped<PlatinumDev.KnittingAIWebAPI.Domain.KnittingProcessorFacade>();

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (!Directory.Exists(app.Environment.WebRootPath))
{
    Directory.CreateDirectory(app.Environment.WebRootPath);
}

// --- Middleware ---
app.UseCors("AllowFrontend");
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

// --- Health и root ---
app.MapGet("/", () => "Knitting MVP API (.NET 8, Minimal APIs)");
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// --- Endpointy проекта ---
app.MapProjectEndpoints();

app.Run();
