using System.Collections.Concurrent;
using PlatinumDev.KnittingAIWebAPI.Domain;

namespace PlatinumDev.KnittingAIWebAPI.Infrastructure;

/// <summary>
/// Простое потокобезопасное хранилище проектов в памяти — подходит для MVP.
/// </summary>
public class InMemoryProjectRepository : IProjectRepository
{
    private readonly ConcurrentDictionary<Guid, PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject> _db = new();

    public void Save(PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject project) => _db[project.Id] = project;

    public PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject? Load(Guid id) => _db.TryGetValue(id, out var p) ? p : null;

    public IEnumerable<PlatinumDev.KnittingAIWebAPI.Domain.KnittingProject> GetAll()
        => _db.Values.OrderByDescending(p => p.CreatedUtc);
}
