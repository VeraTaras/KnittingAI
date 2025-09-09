using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlatinumDev.KnittingAIWebAPI.Domain.Schemes;
using PlatinumDev.KnittingAIWebAPI.Infrastructure;

namespace PlatinumDev.KnittingAIWebAPI.Domain;

/// <summary>
/// Фасад, объединяющий шаги обработки:
/// 1) запуск модели,
/// 2) генерация схем,
/// 3) сборка проекта,
/// 4) сохранение/загрузка проектов.
/// </summary>
public class KnittingProcessorFacade
{
    private readonly PlatinumDev.KnittingAIWebAPI.Infrastructure.IModelRunner _modelRunner;
    private readonly PlatinumDev.KnittingAIWebAPI.Infrastructure.IProjectRepository _repo;

    public KnittingProcessorFacade(PlatinumDev.KnittingAIWebAPI.Infrastructure.IModelRunner modelRunner, PlatinumDev.KnittingAIWebAPI.Infrastructure.IProjectRepository repo)
    {
        _modelRunner = modelRunner;
        _repo = repo;
    }

    // 1. Запуск модели
    public ModelOutputData AnalyzeImage(Stream image)
        => _modelRunner.RunModel(image);

    // 2. Генерация схем (в реальном проекте подменишь фабрикой парсеров)
    public List<IScheme> GenerateSchemes(ModelOutputData modelData)
    {
        var list = new List<IScheme>();

        // Пример: простая текстовая «инструкция» на основе ответа модели
        list.Add(new TextKnittingScheme
        {
            Content = $"Output: {modelData.OutputPath}, confidence={modelData.Confidence:0.###}"
        });

        // Пример: символическая схема как SVG-заглушка (сюда можно подставить реальный SVG)
        list.Add(new SymbolicKnittingScheme
        {
            SvgData = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 100 100\"><text x=\"10\" y=\"50\">symbolic</text></svg>"
        });

        // Пиксельную схему (PNG) можно добавлять по необходимости:
        // list.Add(new PixelKnittingScheme { PngBytes = ... });

        return list;
    }

    // 3. Сборка проекта
    public KnittingProject AssembleProject(List<IScheme> schemes, string name)
    {
        return new KnittingProject
        {
            Name = name,
            TextScheme = schemes.OfType<TextKnittingScheme>().FirstOrDefault(),
            SymbolicScheme = schemes.OfType<SymbolicKnittingScheme>().FirstOrDefault(),
            PixelScheme = schemes.OfType<PixelKnittingScheme>().FirstOrDefault()
        };
    }

    // 4. Сохранение/загрузка
    public void SaveProject(PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject project) => _repo.Save(project);
    public PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject? Load(Guid id) => _repo.Load(id);
    public IEnumerable<PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject> GetAll() => _repo.GetAll();
}

/// <summary>
/// Контракт запуска AI-модели. Инфраструктурные реализации (HTTP/Docker) должны его реализовать.
/// </summary>
public interface IModelRunner
{
    ModelOutputData RunModel(Stream image);
}

/// <summary>
/// Контракт репозитория проектов. Можно иметь in-memory/БД реализации.
/// </summary>
public interface IProjectRepository
{
    void Save(PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject project);
    PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject? Load(Guid id);
    IEnumerable<PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject> GetAll();
}
