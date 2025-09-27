using System.Collections.Concurrent;
using PlatinumDev.KnittingAIWebAPI.Domain;

namespace PlatinumDev.KnittingAIWebAPI.Infrastructure;

/// <summary>
/// Proste, bezpieczne dla wątków repozytorium projektów w pamięci — odpowiednie dla MVP.
/// </summary>
public class InMemoryProjectRepository : IProjectRepository
{
    private readonly ConcurrentDictionary<Guid, PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject> _db = new();
    private int _counter = 0;

    public void Save(KnittingProject project)
    {
        _counter++;
        project.Name = $"Projekt {_counter} ({DateTime.Now:yyyy-MM-dd HH:mm:ss})";
        _db[project.Id] = project;
    }

    public KnittingProject? Load(Guid id)
    {
        _db.TryGetValue(id, out var project);
        return project;
    }

    public IEnumerable<KnittingProject> GetAll()
    {
        return _db.Values
                        .OrderBy(p => p.CreatedAt) // сортировка по дате
                        .ToList();
    }
}