using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlatinumDev.KnittingAIWebAPI.Domain.Schemes;
using PlatinumDev.KnittingAIWebAPI.Infrastructure;

namespace PlatinumDev.KnittingAIWebAPI.Domain;

/// <summary>
/// Fasada łącząca kroki przetwarzania:
/// 1) uruchomienie modelu,
/// 2) generowanie schematów,
/// 3) składanie projektu,
/// 4) zapis/odczyt projektów.
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

    // 1. Uruchomienie modelu
    public ModelOutputData AnalyzeImage(Stream image)
        => _modelRunner.RunModel(image);

    // 2. Generowanie schematów (w realnym projekcie zamienisz na fabrykę parserów)
    public List<IScheme> GenerateSchemes(ModelOutputData modelData)
    {
        var list = new List<IScheme>();

        // Przykład: prosta tekstowa „instrukcja” na podstawie odpowiedzi modelu
        list.Add(new TextKnittingScheme
        {
            Content = $"Output: {modelData.OutputPath}, confidence={modelData.Confidence:0.###}"
        });

        // Przykład: schemat symboliczny jako SVG-atrapa (tutaj można podstawić realne SVG)
        list.Add(new SymbolicKnittingScheme
        {
            SvgData = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 100 100\"><text x=\"10\" y=\"50\">symbolic</text></svg>"
        });

        // Schemat pikselowy (PNG) można dodawać według potrzeb:
        // list.Add(new PixelKnittingScheme { PngBytes = ... });

        return list;
    }

    // 3. Składanie projektu
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

    // 4. Zapis/odczyt
    public void SaveProject(PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject project) => _repo.Save(project);
    public PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject? Load(Guid id) => _repo.Load(id);
    public IEnumerable<PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject> GetAll() => _repo.GetAll();
}

/// <summary>
/// Kontrakt uruchamiania modelu AI. Implementacje infrastrukturalne (HTTP/Docker) muszą go zaimplementować.
/// </summary>
public interface IModelRunner
{
    ModelOutputData RunModel(Stream image);
}

/// <summary>
/// Kontrakt repozytorium projektów. Można mieć implementacje in-memory / baza danych.
/// </summary>
public interface IProjectRepository
{
    void Save(PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject project);
    PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject? Load(Guid id);
    IEnumerable<PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject> GetAll();
}
